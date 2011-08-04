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
        // POST /rhoconnect/authenticate
        public ActionResult authenticate()
        {
            if (Request.ContentType != "application/json")
                return Json("Error");

            String json;
            using (var sr = new StreamReader(Request.InputStream))
            {
                json = sr.ReadToEnd();
            }

            // then deserialize json as instance of dynamically created wrapper class 
            Hashtable req = new JavaScriptSerializer().Deserialize<Hashtable>(json);

            String another_json = new JavaScriptSerializer().Serialize(req);

            return new ContentResult
            {
                Content = "",
                ContentType = "text/plain"
            };
        }

        // POST /rhoconnect/query
        public ActionResult query()
        {
            if (Request.ContentType != "application/json")
                return Json("Error");

            String json;
            using (var sr = new StreamReader(Request.InputStream))
            {
                json = sr.ReadToEnd();
            }

            // then deserialize json as instance of dynamically created wrapper class 
            Hashtable req = new JavaScriptSerializer().Deserialize<Hashtable>(json);
            string controllerName = (String)req["resource"];

            // Instantiate the controller and call Execute 
            IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
            RequestContext context = ControllerContext.RequestContext;
            IController controller = factory.CreateController(context, controllerName);
            if (controller == null)
            {
                return Json("null controller");
            }
            // now we have a controller
            RhoconnectCRUD myCRUD = (RhoconnectCRUD)controller;
            return myCRUD.rhoconnect_query_objects();
        }

        public ActionResult create()
        {
            if (Request.ContentType != "application/json")
                return Json("Error");

            String json;
            using (var sr = new StreamReader(Request.InputStream))
            {
                json = sr.ReadToEnd();
            }

            // then deserialize json as instance of dynamically created wrapper class 
            Hashtable req = new JavaScriptSerializer().Deserialize<Hashtable>(json);
            string controllerName = (String)req["resource"];
            string objectString = new JavaScriptSerializer().Serialize(req["attributes"]);

            //string controllerName = "Movie";



            // Instantiate the controller and call Execute 
            IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
            RequestContext context = ControllerContext.RequestContext;
            Controller controller = (Controller)factory.CreateController(context, controllerName);
            RhoconnectCRUD rh_controller = (RhoconnectCRUD)controller;
            if (controller == null)
            {
                return Json("null controller");
            }

            return rh_controller.rhoconnect_create(objectString);
            //    Debug.WriteLine("here is the object: " + requestBody);
            //byte[] byteData = UTF8Encoding.UTF8.GetBytes(requestBody);
            //request.ContentLength = byteData.Length;
            //using (Stream requestStream = request.GetRequestStream())
            //{
            //    requestStream.Write(byteData, 0, byteData.Length);
            //}
            //}

            //ControllerContext ccontext = new ControllerContext(ControllerContext.RequestContext, controller);
            //ControllerActionInvoker actionInvoker = (ControllerActionInvoker)controller.ActionInvoker;
            //ControllerDescriptor controllerDescriptor = new ReflectedControllerDescriptor(controller.GetType());
            //ActionDescriptor actionDescriptor = controllerDescriptor.FindAction(ccontext, "Create");
            //IDictionary<string, object> parameters = GetParameterValues(controller, actionInvoker, ControllerContext, actionDescriptor);
            //Object res = actionDescriptor.Execute(ccontext, parameters);
            //controller.ActionInvoker.InvokeAction(controller.ControllerContext, "Create");
            //return Json("");
        }

        public ActionResult update()
        {
            if (Request.ContentType != "application/json")
                return Json("Error");

            String json;
            using (var sr = new StreamReader(Request.InputStream))
            {
                json = sr.ReadToEnd();
            }

            // then deserialize json as instance of dynamically created wrapper class 
            Hashtable req = new JavaScriptSerializer().Deserialize<Hashtable>(json);
            string controllerName = (String)req["resource"];

            Dictionary<string, object> changes = (Dictionary<string, object>)req["attributes"];
            //string controllerName = "Movie";



            // Instantiate the controller and call Execute 
            IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
            RequestContext context = ControllerContext.RequestContext;
            Controller controller = (Controller)factory.CreateController(context, controllerName);
            RhoconnectCRUD rh_controller = (RhoconnectCRUD)controller;
            if (controller == null)
            {
                return Json("null controller");
            }

            return rh_controller.rhoconnect_update(changes);
            //    Debug.WriteLine("here is the object: " + requestBody);
            //byte[] byteData = UTF8Encoding.UTF8.GetBytes(requestBody);
            //request.ContentLength = byteData.Length;
            //using (Stream requestStream = request.GetRequestStream())
            //{
            //    requestStream.Write(byteData, 0, byteData.Length);
            //}
            //}

            //ControllerContext ccontext = new ControllerContext(ControllerContext.RequestContext, controller);
            //ControllerActionInvoker actionInvoker = (ControllerActionInvoker)controller.ActionInvoker;
            //ControllerDescriptor controllerDescriptor = new ReflectedControllerDescriptor(controller.GetType());
            //ActionDescriptor actionDescriptor = controllerDescriptor.FindAction(ccontext, "Create");
            //IDictionary<string, object> parameters = GetParameterValues(controller, actionInvoker, ControllerContext, actionDescriptor);
            //Object res = actionDescriptor.Execute(ccontext, parameters);
            //controller.ActionInvoker.InvokeAction(controller.ControllerContext, "Create");
            //return Json("");
        }

        public ActionResult delete()
        {
            if (Request.ContentType != "application/json")
                return Json("Error");

            String json;
            using (var sr = new StreamReader(Request.InputStream))
            {
                json = sr.ReadToEnd();
            }

            // then deserialize json as instance of dynamically created wrapper class 
            Hashtable req = new JavaScriptSerializer().Deserialize<Hashtable>(json);
            string controllerName = (String)req["resource"];

            Dictionary<string, object> changes = (Dictionary<string, object>)req["attributes"];
            //string controllerName = "Movie";



            // Instantiate the controller and call Execute 
            IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
            RequestContext context = ControllerContext.RequestContext;
            Controller controller = (Controller)factory.CreateController(context, controllerName);
            RhoconnectCRUD rh_controller = (RhoconnectCRUD)controller;
            if (controller == null)
            {
                return Json("null controller");
            }

            return rh_controller.rhoconnect_delete(changes["id"]);
        }

        private IModelBinder GetModelBinder(RhoconnectCRUD rh_controller, ParameterDescriptor parameterDescriptor)
        {
            // look on the parameter itself, then look in the global table
            return rh_controller.rhoconnect_model_binder(parameterDescriptor.ParameterType);
        }

        // private static Predicate<string> GetPropertyFilter(ParameterDescriptor parameterDescriptor)
        //{
        //  ParameterBindingInfo bindingInfo = parameterDescriptor.BindingInfo;
        // return propertyName => BindAttribute.IsPropertyAllowed(propertyName, bindingInfo.Include.ToArray(), bindingInfo.Exclude.ToArray());
        //}

        protected virtual object GetParameterValue(Controller controller,
                                                   ControllerActionInvoker actionInvoker,
                                                   ControllerContext controllerContext, ParameterDescriptor parameterDescriptor)
        {
            // collect all of the necessary binding properties 
            Type parameterType = parameterDescriptor.ParameterType;
            RhoconnectCRUD rh_controller = (RhoconnectCRUD)controller;
            IModelBinder binder = GetModelBinder(rh_controller, parameterDescriptor);
            IValueProvider valueProvider = controllerContext.Controller.ValueProvider;
            string parameterName = parameterDescriptor.BindingInfo.Prefix ?? parameterDescriptor.ParameterName;
            //Predicate<string> propertyFilter = GetPropertyFilter(parameterDescriptor);

            // finally, call into the binder
            ModelBindingContext bindingContext = new ModelBindingContext()
            {
                FallbackToEmptyPrefix = (parameterDescriptor.BindingInfo.Prefix == null), // only fall back if prefix not specified 
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, parameterType),
                ModelName = parameterName,
                ModelState = controllerContext.Controller.ViewData.ModelState,
                PropertyFilter = null,
                ValueProvider = valueProvider
            };

            object result = binder.BindModel(controllerContext, bindingContext);
            return result ?? parameterDescriptor.DefaultValue;
        }

        protected virtual IDictionary<string, object> GetParameterValues(Controller controller,
                                                                         ControllerActionInvoker actionInvoker,
                                                                         ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            Dictionary<string, object> parametersDict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            ParameterDescriptor[] parameterDescriptors = actionDescriptor.GetParameters();

            foreach (ParameterDescriptor parameterDescriptor in parameterDescriptors)
            {
                parametersDict[parameterDescriptor.ParameterName] = GetParameterValue(controller, actionInvoker, controllerContext, parameterDescriptor);
            }
            return parametersDict;
        }
    }
}
