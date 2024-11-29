
namespace YadiYad.Pro.Web.Models
{
    public class ModalFooterModel
    {
        public string SubmitButtonText { get; set; }
        public string CancelButtonText { get; set; }
        public string RejectButtonText { get; set; }
        public string SubmitButtonID { get; set; } = "btn-submit";
        public string CancelButtonID { get; set; } = "btn-cancel";
        public string RejectButtonID { get; set; } = "btn-reject";
        public bool ShowSubmitButton { get; set; } = true;
        public bool ShowRejectButton { get; set; } = false;
    }
}