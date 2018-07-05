using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLMClient.Config
{
    class SaveData
    {
        public bool rememberId { get; set; }
        public bool rememberPassword { get; set; }
        public bool autoLogin { get; set; }
        public string saveId { get; set; }
        public string savePass { get; set; }

        public SaveData(bool rememberId, bool rememberPassword, bool autoLogin,
            string saveId, string savePass)
        {
            this.rememberId = rememberId;
            this.rememberPassword = rememberPassword;
            this.autoLogin = autoLogin;
            this.saveId = saveId;
            this.savePass = savePass;
        }
    }
}
