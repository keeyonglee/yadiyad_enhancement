using YadiYad.Pro.Web.Models.Enums;

namespace YadiYad.Pro.Web.Models
{
    public class BootstrapModel
    {
        public string ID { get; set; }
        public string AreaLabeledId { get; set; }
        public ModalSize Size { get; set; }
        public string Message { get; set; }
        public string AdditionalClass { get; set; }
        public bool IsVerticalAlign { get; set; }
        public string ModalSizeClass
        {
            get
            {
                switch (this.Size)
                {
                    case ModalSize.Small:
                        return "modal-sm";
                    case ModalSize.Large:
                        return "modal-lg";
                    case ModalSize.XLarge:
                        return "modal-xl";
                    case ModalSize.Medium:
                    default:
                        return "";
                }
            }
        }
    }
}