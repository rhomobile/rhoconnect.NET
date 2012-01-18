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

        private IRhoconnectCRUD instantiate_controller(String controllerName)
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
            return (IRhoconnectCRUD)controller;
        }

        // POST /rhoconnect/authenticate
        public ActionResult authenticate()
        {
            // authenticate here
            String returnValue;
            try
            {
                Hashtable reqHash = deserialize_request();
                // in OK responses , returnValue is authenticated username
                returnValue = (String)reqHash["login"];
                Response.StatusCode = 200;
                bool authOK = RhoconnectNET.Helpers.rhoconnect_authenticate(ref returnValue, (String)reqHash["password"], reqHash);
                if (!authOK)
                    throw new SystemException("Authentication has failed");
            }
            catch (SystemException exc)
            {
                // set the response code to 401 and return error message
                returnValue = exc.Message;
                Response.StatusCode = 401;
            }

            return new ContentResult
            {
                Content = returnValue,
                ContentType = "text/plain"
            };
        }

        // POST /rhoconnect/query
        public ActionResult query()
        {
            try {
                Hashtable reqHash = deserialize_request();
                IRhoconnectCRUD controller = instantiate_controller((String)reqHash["resource"]);
                return controller.rhoconnect_query_objects((String)reqHash["user_id"]);
            }
            catch
            {
                Response.StatusCode = 500;
            }

            return Json("");
        }

        // POST /rhoconnect/create
        public ActionResult create()
        {
            try {
                Hashtable reqHash = deserialize_request();
                string objectString = new JavaScriptSerializer().Serialize(reqHash["attributes"]);

                IRhoconnectCRUD controller = instantiate_controller((String)reqHash["resource"]); 
                return controller.rhoconnect_create(objectString, (String)reqHash["user_id"]);
            }
            catch
            {
                Response.StatusCode = 500;
            }

            return Json("");
        }

        // POST /rhoconnect/update
        public ActionResult update()
        {
            try {
                Hashtable reqHash = deserialize_request();
                Dictionary<string, object> changes = (Dictionary<string, object>)reqHash["attributes"];

                IRhoconnectCRUD rh_controller = instantiate_controller((String)reqHash["resource"]);
                return rh_controller.rhoconnect_update(changes, (String)reqHash["user_id"]);
            }
            catch
            {
                Response.StatusCode = 500;
            }

            return Json("");
        }

        // POST /rhoconnect/delete
        public ActionResult delete()
        {
            try {
                Hashtable reqHash = deserialize_request();
                Dictionary<string, object> deletes = (Dictionary<string, object>)reqHash["attributes"];

                IRhoconnectCRUD rh_controller = instantiate_controller((String)reqHash["resource"]);
                return rh_controller.rhoconnect_delete(deletes["id"], (String)reqHash["user_id"]);
            }
            catch
            {
                Response.StatusCode = 500;
            }

            return Json("");
        }
    }
}
