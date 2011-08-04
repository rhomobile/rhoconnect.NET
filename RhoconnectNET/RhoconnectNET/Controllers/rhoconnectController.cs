using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Mvc.Resources;
using System.Web.Mvc;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Web.Routing;
using RhoconnectNET;

namespace RhoconnectNET.Controllers
{
    public class rhoconnectController : Controller
    {
        private Hashtable deserialize_request()
        {
            if (Request.ContentType != "application/json")
                throw new SystemException("Content-Type must be set to 'application/json'");

            String json;
            using (var sr = new StreamReader(Request.InputStream))
            {
                json = sr.ReadToEnd();
            }

            // then deserialize json as instance of dynamically created wrapper class 
            return new JavaScriptSerializer().Deserialize<Hashtable>(json);
        }

        private RhoconnectCRUD instantiate_controller(String controllerName)
        {
            // Instantiate the controller and call Execute 
            IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
            RequestContext context = ControllerContext.RequestContext;
            IController controller = factory.CreateController(context, controllerName);
            if (controller == null)
            {
                throw new SystemException("Cannot instantiate controller: " + controllerName);
            }
            // now we have a controller
            return (RhoconnectCRUD)controller;
        }

        // POST /rhoconnect/authenticate
        public ActionResult authenticate()
        {
            // authenticate here
            try
            {
                Hashtable reqHash = deserialize_request();
                Response.StatusCode = 200;
            }
            catch
            {
                // set the response code to 500
                Response.StatusCode = 500;
            }

            return new ContentResult
            {
                Content = "",
                ContentType = "text/plain"
            };
        }

        // POST /rhoconnect/query
        public ActionResult query()
        {
            try {
                Hashtable reqHash = deserialize_request();
                RhoconnectCRUD controller = instantiate_controller((String)reqHash["resource"]); 
                return controller.rhoconnect_query_objects();
            }
            catch
            {
                Response.StatusCode = 500;
            }

            return Json("");
        }

        public ActionResult create()
        {
            try {
                Hashtable reqHash = deserialize_request();
                string objectString = new JavaScriptSerializer().Serialize(reqHash["attributes"]);

                RhoconnectCRUD controller = instantiate_controller((String)reqHash["resource"]); 
                return controller.rhoconnect_create(objectString);
            }
            catch
            {
                Response.StatusCode = 500;
            }

            return Json("");
        }

        public ActionResult update()
        {
            try {
                Hashtable reqHash = deserialize_request();
                Dictionary<string, object> changes = (Dictionary<string, object>)reqHash["attributes"];

                RhoconnectCRUD rh_controller = instantiate_controller((String)reqHash["resource"]); 
                return rh_controller.rhoconnect_update(changes);
            }
            catch
            {
                Response.StatusCode = 500;
            }

            return Json("");
        }

        public ActionResult delete()
        {
            try {
                Hashtable reqHash = deserialize_request();
                Dictionary<string, object> deletes = (Dictionary<string, object>)reqHash["attributes"];

                RhoconnectCRUD rh_controller = instantiate_controller((String)reqHash["resource"]); 
                return rh_controller.rhoconnect_delete(deletes["id"]);
            }
            catch
            {
                Response.StatusCode = 500;
            }

            return Json("");
        }
    }
}
