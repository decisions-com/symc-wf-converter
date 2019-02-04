using LogicBase.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBaseProjectConversionUtility
{
    public class FakeDesignerPrefs : IDesignerPreferences
    {
        #region IDesignerPreferences Members

        public bool AddBusinessModelToNewProjects
        {
            get { return true; }
        }

        public bool AddDefaultSwimLane
        {
            get { return true; }
        }

        public bool BusinessModelErrorsAreWarnings
        {
            get { return true; }
        }

        public int CacheTimeoutInMin
        {
            get
            {
                return 0;
            }
            set
            {
                return;
            }
        }

        public string DefaultDeployDirectory
        {
            get { return null; }
        }

        public string DefaultXmlNameSpace
        {
            get
            {
                return null;
            }
            set
            {
                return;
            }
        }

        public bool EnableEnsembleIntegration
        {
            get { return true; }
        }

        public bool EnableLocalizationSupport
        {
            get
            {
                return false;
            }
            set
            {
                return;
            }
        }

        public bool IncludeCustomLibs
        {
            get { return true; }
        }

        public string LocalDeploymentRoot
        {
            get { return ""; }
        }

        public bool PrePopulateConnectionStringsOnGeneratedComponents
        {
            get { return true; }
        }

        public bool PromptForBasicFormData
        {
            get { return true; }
            set
            {
                return;
            }
        }

        public bool RemoveDeployDirectoryItself
        {
            get { return true; }
        }

        public bool ShowBusinessModel
        {
            get { return true; }
        }

        public object this[string name]
        {
            get { return ""; }
        }

        #endregion
    }
}
