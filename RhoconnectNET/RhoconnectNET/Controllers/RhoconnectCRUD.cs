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
    public interface RhoconnectCRUD
    {
        JsonResult rhoconnect_query_objects();
        String rhoconnect_partition();
        String rhoconnect_id_field();
        ActionResult rhoconnect_create(String objJson);
        ActionResult rhoconnect_update(Dictionary<string, object> changes);
        ActionResult rhoconnect_delete(Object objId);
        IModelBinder rhoconnect_model_binder(Type parameterType);
    }
}
