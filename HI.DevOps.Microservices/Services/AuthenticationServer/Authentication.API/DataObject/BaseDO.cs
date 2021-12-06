using System.Collections.Generic;

namespace Hi.DevOps.Authentication.API.DataObject
{
    public class BaseDo
    {
        public BaseDo()
        {
            ErrorListDo = new List<ErrorDo>();
        }


        public List<ErrorDo> ErrorListDo { get; set; }
    }
}