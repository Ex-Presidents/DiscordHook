using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulator.Interfaces;

namespace Modulator.API
{
    public class Configuration<T> where T : IConfiguration
    {
        #region Properties
        public static T Instance
        {
            get
            {
                
            }
        }
        #endregion
    }
}
