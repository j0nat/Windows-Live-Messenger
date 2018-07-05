using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;

namespace WLMClient.UI.Data
{
    class ContactListEntryData
    {
        public RichTextBox richTextBox { get; set; }
        public Image image { get; set; }
        public TextBlock name { get; set; }
        public TextBlock comment { get; set; }

        public ContactListEntryData(RichTextBox richTextBox, Image image, TextBlock name, TextBlock comment)
        {
            this.richTextBox = richTextBox;
            this.image = image;
            this.name = name;
            this.comment = comment;
        }
    }
}
