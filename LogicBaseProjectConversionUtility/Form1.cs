using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using LogicBase.Components.Default.DateHandling;
using LogicBase.Components.Default.FlowControl;
using LogicBase.Components.Default.Process;
using LogicBase.Components.Default.Rules;
using LogicBase.Core;
using LogicBase.Core.Data;
using LogicBase.Framework.Loader;
using Microsoft.Win32;
using GetItemByIndex = LogicBase.Components.Default.Process.GetItemByIndex;
using System.ServiceModel;
using LogicBaseProjectConversionUtility.ProjectConversionService;
using LogicBase.Core.Data.DataTypes;
using System.Linq;
using LogicBase.Components.Default.Logging;
using LogicBase.Components.FormBuilder;
using LogicBaseProjectConversionUtility;
using System.Xml.Serialization;
using System.Xml;

namespace LogicBaseToDecisionsProjectConversionTool
{
    public partial class Form1 : Form
    {
        delegate void StringParameterDelegate(string message);

        public static Dictionary<Type, LogicBaseStepConverter> ConverterFor = new Dictionary<Type, LogicBaseStepConverter>();

        public static Dictionary<string, int> dictAllStepsCount = new Dictionary<string, int>();

        private bool librariesInitialized = false;

        static Form1()
        {
            ConverterFor[typeof(StartComponent)] = new StartStepConvertor();
            ConverterFor[typeof(EndComponent)] = new EndStepConvertor();

            //ConverterFor[typeof(InsertDataComponent)] = new InsertDataStepConverter();
            //ConverterFor[typeof(AssignDataComponent)] = new AssignDataStepConverter();

            //ConverterFor[typeof(TextEqualsRule)] = new TextEqualsStepConverter();

            ConverterFor[typeof(EqualsRule)] = new EqualsRuleConvertor();

            ConverterFor[typeof(MatchesRule)] = new MatchesLogicBaseStepConverter();

            //ConverterFor[typeof(TextExists)] = new TextExistsStepConverter();

            ConverterFor[typeof(BuildTextFromElements)] = new CreateTextFromArrayConvertor();

            ConverterFor[typeof(VariableExistsRule)] = new VariableExistsRuleConvertor();

            ConverterFor[typeof(GetNumberFromString)] = new ParseNumberConvertor();

            ConverterFor[typeof(ConvertStringToDate)] = new ParseDateConvertor();

            ConverterFor[typeof(IndexOfItemInCollection)] = new IndexOfItemInListConvertor();

            ConverterFor[typeof(GetItemByIndex)] = new ItemByIndexConvertor();

            ConverterFor[typeof(GoToComponentByName)] = new GoToComponentConvertor();

            ConverterFor[typeof(CompareNumbersRule)] = new CompareNumbersConvertor();

            ConverterFor[typeof(GetCurrentDate)] = new GetCurrentDateConvertor();

            ConverterFor[typeof(ToShortDateString)] = new ShortDateConvertor();

            ConverterFor[typeof(LinkedModelComponent)] = new LinkedModelConvertor();

            ConverterFor[typeof(EmbeddedModelComponent)] = new EmbeddedModelConvertor();

            // ConverterFor[typeof(FormBuilderComponent)] = new FormBuilderStepConverter();

            ConverterFor[typeof(Symantec.Workflow.Components.Platform.InitializeWorkflowSettings)] = new CreateNSCredentialsConverter();

            ConverterFor[typeof(LogicBase.Components.Default.Communication.SendEmailComponent)] = new SendEmailStepConverter();

            ConverterFor[typeof(CreateLogEntryComponent)] = new CreateLogEntryComponentConverter();

            ConverterFor[typeof(InsertDataComponent)] = new InsertDataStepConverter();
        }

    
        public Form1()
        {
         
            InitializeComponent();
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Multiselect = false;
            TryToFindWorkflowInstallation();

            InitDesignerPrefernecs();

            //textBox1.Text = @"C:\Program Files (x86)\Altiris\Workflow Designer\WorkflowProjects\Wrap Up 2\Wrap Up 2.WebForms";
            //textBox1.Text = @"C:\Program Files (x86)\Altiris\Workflow Designer\WorkflowProjects\WebFormsProject1\WebFormsProject1.WebForms";
            textBox1.Text = @"C:\Program Files (x86)\Altiris\Workflow Designer\WorkflowProjects\AConvertTest\AConvertTest.WebForms";

        }

        private void InitDesignerPrefernecs()
        {
            LogicBase.Core.Utilities.DesignerPreferencesAccessor.SetPreferences(new FakeDesignerPrefs());
        }

       private void TryToFindWorkflowInstallation()
        {
            // Check for the Registry Path
            RegistryKey lbKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\TransparentLogic.com\LogicBase Shared");
            RegistryKey lb64BKey = Registry.LocalMachine.OpenSubKey(@"Software\Wow6432Node\TransparentLogic.com\LogicBase Shared");

            string path = "";

            //try to get path from reg key
            object pathValue = null;
            // Try for the 64bit Key first
            if (lb64BKey != null)
            {
                pathValue = lb64BKey.GetValue("LOGICBASE_SHARED_DIRECTORY");
            }

            if (lbKey != null)
            {
                pathValue = lbKey.GetValue("LOGICBASE_SHARED_DIRECTORY");
            }

            if (pathValue != null)
            {
                path = pathValue as string;
                if (!string.IsNullOrEmpty(path))
                {
                    folderBrowserDialog1.SelectedPath = path;
                    textBox2.Text = path;

                    openFileDialog1.InitialDirectory = path.Substring(0, path.LastIndexOf("\\")) +  "\\WorkflowProjects";
                    return;

                }

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                // Load File
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dr = folderBrowserDialog1.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath;
            }
        }


        // HSO hold this in memory after conversion to decide about uploading.
        private ConvertedProject convertedProject = null;
        private List<ConvertedForm> allForms = new List<ConvertedForm>();

        private void ConvertButtonClicked(object sender, EventArgs e)
        {
            allForms.Clear();

            // Begin Conversion
            richTextBox1.Clear();

            Report("Starting Conversion");
            Report("Loading Workflow Project");

            try
            {
                AbstractOrchestrationProject pd = LoadWorkflowProject();

                Report("Project Loaded");

                ConvertedFlow[] results = ProcessModels(pd);
                
                Report("Done Processing Models");

                convertedProject = new ConvertedProject();
                convertedProject.ProjectName = pd.ProjectSetupData.Name;
                convertedProject.Flows = results;

                convertedProject.Forms = allForms.ToArray();

                // Upload converted project.

            }
            catch (Exception ex)
            {
                Report("The following exception has aborted conversion: {0}", ex.Message);
            }

            File.WriteAllLines("AllStepsCount.txt",
                dictAllStepsCount.Select(x => string.Format("{0},{1}", x.Key, x.Value)).ToArray());

            Report("Work Complete.");
        }

        private ConvertedFlow[] ProcessModels(AbstractOrchestrationProject aop)
        {
            Report("Preparing to process models");
            flowsForProject.Clear();
            projectName = aop.ProjectSetupData.Name;


            foreach (Model m in aop.ProjectSetupData.Models)
            {
                Report("Creating Decisions Flow Named: {0}.{1}", aop.ProjectSetupData.Name, m.ModelName);
                ConvertedFlow newFlow = new ConvertedFlow();
                newFlow.FlowName = m.ModelName;
                newFlow.FlowId = m.ModelID;
                
                if(string.IsNullOrEmpty(m.ParentModelName))
                {
                    newFlow.Tags = new string[] { "Root" };
                }
                
                else
                {
                    newFlow.Tags = new string[] { m.ParentModelName };
                }

                MODEL_NAME_TO_ID_MAP[m.ModelName] = m.ModelID;

                // Setup the flow input data according to the 
                // LogicBase model input data
                IComponentModel myModel = aop.GetComponentModel(m);
                SetupInputDataForFlow(myModel.InputData, newFlow);

                ProcessComponentsFromModelToFlow(myModel, newFlow, aop.ProjectSetupData.Name, m.ModelName, flowsForProject);

                Report("Flow Complete.");

                AddFlowToList(newFlow);
            }
            return flowsForProject.ToArray();
        }

        List<ConvertedFlow> flowsForProject = new List<ConvertedFlow>();
        private void AddFlowToList(ConvertedFlow newFlow)
        {
            flowsForProject.Add(newFlow);
        }

        // Done
        private void SetupInputDataForFlow(DataDefinitionCollection allInputs, ConvertedFlow newFlow, bool filterPP = false)
        {
            List<LogicBaseProjectConversionUtility.ProjectConversionService.DataDefinition> flowInputData = new List<LogicBaseProjectConversionUtility.ProjectConversionService.DataDefinition>();
            foreach (LogicBase.Core.Data.DataDefinition input in allInputs)
            {
                if (input.Name.StartsWith("[ProjectProperties].") || input.Name.StartsWith("[Global].")) {
                    continue;
                }

                try
                {
                    LogicBaseProjectConversionUtility.ProjectConversionService.DataDefinition fid = new LogicBaseProjectConversionUtility.ProjectConversionService.DataDefinition();
                    fid.CanBeNull = input.IsNullAllowed;
                    fid.IsList = input.IsArray;
                    fid.FullTypeName = input.DataType.FullName;
                    fid.Name = input.Name;
                    flowInputData.Add(fid);
                } catch (Exception e)
                {
                    if (input == null)
                    {
                        Report("Tried to parse null input value");
                    } 
                    else
                    {
                        Report("Problem converting input named {0} of type {1}", input.Name, input.DataType == null ? "NULL" : input.DataType.ToString());
                    }

                }
            }
            newFlow.InputData = flowInputData.ToArray();
        }

        private void ProcessComponentsFromModelToFlow(IComponentModel compModel, ConvertedFlow newFlow, string projectName, string modelName, List<ConvertedFlow> allFlows)
        {

            List<ConvertedStep> allSteps = new List<ConvertedStep>();

            foreach (IOrchestrationComponent component in compModel.Components)
            {
                if (component is EmbeddedModelComponent)
                {
                    ConvertedFlow embeddedFlow = new ConvertedFlow();
                    embeddedFlow.FlowName = component.Name;


                    FieldInfo[] fields = component.GetType().GetFields(
                         BindingFlags.NonPublic |
                         BindingFlags.Instance);

                    IComponentModel internalModel = (IComponentModel)fields.First(xx => xx.Name == "_embeddedModel").GetValue(component);

                    SetupInputDataForFlow(internalModel.InputData, embeddedFlow, true);
                    SetupOutputDataForFlow(internalModel.OutputData, embeddedFlow);
                    embeddedFlow.Tags = new string[] {modelName, "Embedded Model"};
                    embeddedFlow.FlowId = internalModel.Id;

                    ProcessComponentsFromModelToFlow(internalModel, embeddedFlow, projectName, component.Name + " Embedded", allFlows);

                    allFlows.Add(embeddedFlow);

                }
                
                ConvertedStep x = CreateFlowStepFromComponent(projectName, component, modelName);

                allSteps.Add(x);
                
            }

            newFlow.Steps = allSteps.ToArray();

            List<ConvertedConnection> stepConnections = new List<ConvertedConnection>();

            // Now that we have created and mapped all the steps we 
            // need to create all of the necessary links
            foreach (IOrchestrationComponent component in compModel.Components)
            {
                Report("Linking {0}", component.Name);
                
                IComponentLink[] links = compModel.GetOutboundLinks(component);
                
                foreach (IComponentLink eachLink in links)
                {
                    string srcPath = eachLink.Path;
                    int srcPortNumber = eachLink.SourcePortNumber + 1;
                    int dstPortNumber = eachLink.DestinationPortNumber - 1;
                    dstPortNumber = dstPortNumber < 0 ? 0 : dstPortNumber;
                    srcPortNumber = srcPortNumber > 3 ? 3 : srcPortNumber;

                    stepConnections.Add(new ConvertedConnection() { 
                        TargetStepId = eachLink.DestinationComponentID,
                        SourceStepId = eachLink.SourceComponentID,
                        TargetPortNumberOnStep = dstPortNumber,
                        SourcePortNumberOnStep = srcPortNumber,
                        PathName = eachLink.Path
                    });
            
                }
            }

            newFlow.Connections = stepConnections.ToArray();
        }

        private ConvertedStep CreateFlowStepFromComponent(string projectName, IOrchestrationComponent component, string modelName)
        {
            Report("Transforming Component {0}", component.Name);

            if (component.GetType().FullName == typeof(FormBuilderComponent).FullName)
            {
                ConvertedForm form = FormBuilderConvertor.GetForm(component);
                
                if (string.IsNullOrEmpty(modelName))
                {
                    form.Tags = new string[] { "Root" };
                }

                else
                {
                    form.Tags = new string[] { modelName };
                }

                form.FormId = component.Id;

                // Add this form to the converted results that we are building.
                allForms.Add(form);

                // And now create the ConvertedStep to represent the form wrapper step on the 
                // decisions side.
                ConvertedStep forWrapperStep = new ConvertedStep();
                forWrapperStep.StepName = component.Name;
                forWrapperStep.X = (int)component.Location.X;
                forWrapperStep.Y = (int)component.Location.Y;
                forWrapperStep.UniqueStepIdForConnections = component.Id;
                
                forWrapperStep.FullTypeName = "FormWrapperStep";
                
                return forWrapperStep;
            }

            ConvertedStep result = new ConvertedStep();

            //build up dictionary of steps and count just so we can know how many are in this flow
            if (dictAllStepsCount.ContainsKey(component.GetType().Name)) {
                dictAllStepsCount[component.GetType().Name] = dictAllStepsCount[component.GetType().Name] + 1;
            }
            else {
                dictAllStepsCount.Add(component.GetType().Name, 1);
            }

            List<string> baseTypeNamesToSkip = new List<string>();
            baseTypeNamesToSkip.Add("AbstractSQLSinglePathComponent");
            //baseTypeNamesToSkip.Add("AbstractSinglePathProcessComponent");
            //baseTypeNamesToSkip.Add("AbstractConnectionStringMultiPathComponent");

            List<string> typeNamesToSkip = new List<string>();
            typeNamesToSkip.Add("LogicBase.Components.Default.Process.MultiPathEmbeddedModelComponent");
            typeNamesToSkip.Add("LogicBase.Components.Default.IO.ReadFile");
            typeNamesToSkip.Add("LogicBase.Components.Default.IterateTextFileLines");
            typeNamesToSkip.Add("LogicBase.Components.Office2003.Word2003ModelComponent");
            typeNamesToSkip.Add("LogicBase.Components.Default.IO.CreateTextFile");
            typeNamesToSkip.Add("LogicBase.Components.Default.Process.SubtractValues");

            List<string> assembliesToSkip = new List<string>();
            assembliesToSkip.Add("Wrap Up 2 Integration.dll");
            assembliesToSkip.Add("Wrap Up Client Side Integration.dll");

            if (baseTypeNamesToSkip.Contains(component.GetType().BaseType.Name) || typeNamesToSkip.Contains(component.GetType().FullName) || assembliesToSkip.Contains(component.GetType().Assembly.ManifestModule.Name))
            {
                result.StepName = component.Name;
                result.UniqueStepIdForConnections = component.Id;
                result.InputData = new StepInput[0];
                result.OutcomeData = new StepOutcome[0];
                result.FullTypeName = "No type";
                result.StepProperties = new StepInput[0];
                result.X = (int)component.Location.X;
                result.Y = (int)component.Location.Y;

                return result;
            }

            result.UniqueStepIdForConnections = component.Id;

            result.FullTypeName = component.GetType().FullName;
            result.StepName = component.Name;
            result.X = (int)component.Location.X;
            result.Y = (int)component.Location.Y;
                
            if (ConverterFor.ContainsKey(component.GetType()))
            {
                result.InputData = ConverterFor[component.GetType()].GetInputs(component);
                result.StepProperties = ConverterFor[component.GetType()].GetProperties(component);
                result.OutcomeData = ConverterFor[component.GetType()].GetOutcomes(component);
            }
            else
            {
                result.InputData = ConvertInputDataDefault(component);
                result.StepProperties = ConvertStepPropertiesDefault(component);
                result.OutcomeData = ConvertOutDataDefault(component);
                // Setup common properties
            }
            return result;

        }

        private StepOutcome[] ConvertOutDataDefault(IOrchestrationComponent component)
        {
            List<StepOutcome> outcomes = new List<StepOutcome>();
            if (typeof(AbstractMultiPathProcessComponent).IsAssignableFrom(component.GetType())) {
                string[] paths = ((AbstractMultiPathProcessComponent)component).GetConnectionPaths();
                foreach (string path in paths) {
                    LogicBase.Core.Data.DataDefinition[] dataParts = null;
                    if (component is IMultiPathDataAdded) {
                        dataParts = ((IMultiPathDataAdded)component).GetAddedData(path);
                    }
                    StepOutcome outcomeToAdd = new StepOutcome() { PathName = path };
                    List<LogicBaseProjectConversionUtility.ProjectConversionService.DataDefinition> outcomeDefs
                        = new List<LogicBaseProjectConversionUtility.ProjectConversionService.DataDefinition>(); 
                    if (dataParts != null) {
                        foreach (LogicBase.Core.Data.DataDefinition oldDef in dataParts) {
                            outcomeDefs.Add(new LogicBaseProjectConversionUtility.ProjectConversionService.DataDefinition() { 
                                FullTypeName = oldDef.DataType.FullName,
                                IsList = oldDef.IsArray,
                                Name = oldDef.Name
                            });
                        }
                    }
                    outcomeToAdd.OutcomeData = outcomeDefs.ToArray();

                    outcomes.Add(outcomeToAdd);
                }
            }
            return outcomes.ToArray();
        }

        private StepInput[] ConvertStepPropertiesDefault(IOrchestrationComponent component)
        {
            return new StepInput[0];
        }

        private StepInput[] ConvertInputDataDefault(IOrchestrationComponent component)
        {
            List<StepInput> allInputs = new List<StepInput>();
            try
            {
                PropertyInfo[] pi = component.GetType().GetProperties();
                foreach (PropertyInfo eachProperty in pi)
                {
                    if (eachProperty.PropertyType == typeof(VariableOrValueDataType))
                    {
                        allInputs.Add(VVDTUtility.ConvertVariableOrValueDataTypeToInputMapping((VariableOrValueDataType)eachProperty.GetValue(component), eachProperty.Name));
                    }
                }
            }
            catch (Exception ex) {
                Report("Exception occured reflecting properties");
            }
            return allInputs.ToArray();
        }

    

        private void SetupOutputDataForFlow(DataDefinitionCollection outputData, ConvertedFlow newFlow)
        {
            // When setting up output data for a flow it actually 
            // needs to be data that is configured on the end steps, 
            // not on the flow object itself.  
            

        }


        private AbstractOrchestrationProject LoadWorkflowProject()
        {
            if (!librariesInitialized)
            {
                InitLibs();
                librariesInitialized = true;
            }

            try
            {
                LogicBase.Core.Utilities.DesignMode.IsInDesignMode = true;
                AbstractOrchestrationProject aop =
                    AbstractOrchestrationProject.OpenProject(new FileInfo(textBox1.Text));
                Report("Loaded abstract orchestration project.");
                return aop;
            } catch (Exception ex)
            {
                Report("An exception prevented the project from being loaded");
                Report(ex.ToString());
                throw ex;
            }
        }

        private void InitLibs()
        {
            Report("Initializing Libraries");

            LoaderBaseClass.InitializeLoader();

        }


        private void Report(string message, params string[] variables)
        {
            if (variables != null && variables.Length > 0)
            {
               WriteMessage(string.Format(message, variables));
            }
            else
            {
                WriteMessage(message);
            }
        }

        private void WriteMessage(string message)
        {
            if (richTextBox1.InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new StringParameterDelegate(WriteMessage), new object[] { message });
                return;
            }
            else
            {
                richTextBox1.Text += string.Format("{0}\r\n", message);
            }
        }

        private string projectName;
        private void SaveToFolderButtonClicked(object sender, EventArgs e)
        {
         
            
            MessageBox.Show("Save");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Writing out the converted project.
            XmlSerializer xsSubmit = new XmlSerializer(typeof(ConvertedProject));
          
            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, convertedProject);
                    File.WriteAllText(".\\projectconverted.xml", sww.ToString()); // Your XML
                }
            }

            Report("Upload converted project");

            ProjectConversionServiceClient client = new ProjectConversionServiceClient("BasicHttpBinding_IProjectConversionService");

            PasswordCredentialsUserContext user = new PasswordCredentialsUserContext();
            user.UserID = "admin@decisions.com";
            user.Password = "admin";

            foreach (ConvertedFlow flow in convertedProject.Flows) {

                foreach (ConvertedStep eachStep in flow.Steps) {
                    if (string.IsNullOrEmpty(eachStep.FullTypeName) == true) {
                        Report("{0} has no type", eachStep.StepName);
                    }
                }

            }

            client.UploadConvertedProject(user, convertedProject);

            Report("All done, snip-snap!");
        }

        private static Dictionary<string, string> MODEL_NAME_TO_ID_MAP = new Dictionary<string, string>();
        internal static object GetIdForModel(string modelName)
        {
            if (MODEL_NAME_TO_ID_MAP.ContainsKey(modelName))
            {
                return MODEL_NAME_TO_ID_MAP[modelName];
            }
            else {
                return modelName;
            }
        }
    }

}
