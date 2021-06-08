namespace BeanBag.Models
{
    /*
  * This class is used to get and set all the variables related to Error checking on pages
  */
    public class ErrorViewModel
    {
        public string RequestId=null;

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}