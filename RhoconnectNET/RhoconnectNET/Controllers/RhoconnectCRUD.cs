using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Mvc;
using System.Collections;
using System.IO;
using System.Diagnostics;

namespace RhoconnectNET.Controllers
{
    public interface IRhoconnectCRUD
    {
        JsonResult rhoconnect_query_objects(String partition);
        ActionResult rhoconnect_create(String objJson, String partition);
        ActionResult rhoconnect_update(Dictionary<string, object> changes, String partition);
        ActionResult rhoconnect_delete(Object objId, String partition);
    }
}
