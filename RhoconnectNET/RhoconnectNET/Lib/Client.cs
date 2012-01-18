using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Net;
using System.Text;
using System.IO;
using System.Collections;
using System.Reflection;

namespace RhoconnectNET
{
    public class Client
    {
        private static String _endpoint_url;
        private static String _api_token;

        public static bool set_app_endpoint(String endpoint,
                                            String app_endpoint,
                                            String api_token,
                                            RhoconnectNET.Helpers.rhoAuthHandler auth_handler)
        {
            try
            {
                _endpoint_url = endpoint;
                _api_token = api_token;
                Helpers._auth_handler = auth_handler;

                Hashtable reqHash = new Hashtable();
                reqHash.Add("api_token", _api_token);
                Hashtable attrHash = new Hashtable();
                attrHash.Add("adapter_url", app_endpoint);
                reqHash.Add("attributes", attrHash);

                JavaScriptSerializer js = new JavaScriptSerializer();
                string requestBody = js.Serialize(reqHash);

                return process_request("save_adapter", requestBody);
            }
            catch (SystemException)
            {
                return false;
            }
        }

        public static bool notify_on_create(String partition,
                                            Object created_obj)
        {
            String source_name = created_obj.GetType().Name;
            String obj_id = get_id_property(created_obj);

            Hashtable objects = new Hashtable();
            objects.Add(obj_id, created_obj);
            return notify_on_create(source_name, partition, objects);
        }

        public static bool notify_on_create(String partition,
                                            String id_property,
                                            Object created_obj)
        {
            String source_name = created_obj.GetType().Name;
            String obj_id = get_id_property(id_property, created_obj);

            Hashtable objects = new Hashtable();
            objects.Add(obj_id, created_obj);
            return notify_on_create(source_name, partition, objects);
        }

        public static bool notify_on_create(String source_name,
                                            String partition,
                                            String id_property,
                                            Object created_obj)
        {
            String obj_id = get_id_property(id_property, created_obj);

            Hashtable objects = new Hashtable();
            objects.Add(obj_id, created_obj);
            return notify_on_create(source_name, partition, objects);
        }
        
        public static bool notify_on_create(String source_name, String partition,
                                            Hashtable objects)
        {
            Hashtable reqHash = new Hashtable();
            reqHash.Add("objects", objects); 
            return send_objects("push_objects", source_name, partition, reqHash);
        }

        public static bool notify_on_update(String partition,
                                            Object updated_obj)
        {
            String source_name = updated_obj.GetType().Name;
            String obj_id = get_id_property(updated_obj);

            Hashtable objects = new Hashtable();
            objects.Add(obj_id, updated_obj);
            return notify_on_update(source_name, partition, objects);
        }

        public static bool notify_on_update(String partition,
                                            String id_property,
                                            Object updated_obj)
        {
            String source_name = updated_obj.GetType().Name;
            String obj_id = get_id_property(id_property, updated_obj);

            Hashtable objects = new Hashtable();
            objects.Add(obj_id, updated_obj);
            return notify_on_update(source_name, partition, objects);
        }

        public static bool notify_on_update(String source_name,
                                            String partition,
                                            String id_property,
                                            Object updated_obj)
        {
            String obj_id = get_id_property(id_property, updated_obj);

            Hashtable objects = new Hashtable();
            objects.Add(obj_id, updated_obj);
            return notify_on_update(source_name, partition, objects);
        }

        public static bool notify_on_update(String source_name, String partition, Hashtable objects)
        {
            Hashtable reqHash = new Hashtable();
            reqHash.Add("objects", objects);
            return send_objects("push_objects", source_name, partition, reqHash);
        }

        public static bool notify_on_delete(Object deleted_obj, String partition)
        {
            String source_name = deleted_obj.GetType().Name;
            String obj_id = get_id_property(deleted_obj);

            return notify_on_delete(source_name, partition, obj_id);
        }

        public static bool notify_on_delete(String source_name, String partition, Object id)
        {
            Hashtable reqHash = new Hashtable();
            Object[] deletes = new Object[1] { id };
            reqHash.Add("objects", deletes);

            return send_objects("push_deletes", source_name, partition, reqHash);
        }

        public static bool notify_on_delete(String source_name, String partition, int id)
        {
            return notify_on_delete(source_name, partition, id.ToString());
        }

        private static bool send_objects(String method, String source_name, String partition,
                                         Hashtable reqHash)
        {
            // add meta information
            reqHash.Add("api_token", _api_token);
            reqHash.Add("source_id", source_name);
            reqHash.Add("user_id", partition);

            JavaScriptSerializer js = new JavaScriptSerializer();
            string requestBody = js.Serialize(reqHash);

            return process_request(method, requestBody);
        }

        private static bool process_request(String method, String body)
        {
            Uri address = new Uri(_endpoint_url + "/api/source/" + method);
            HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Method = "POST";

            byte[] byteData = UTF8Encoding.UTF8.GetBytes(body);
            request.ContentLength = byteData.Length;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(byteData, 0, byteData.Length);
            }
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                ;
            }

            return true;
        }

        private static String get_id_property(Object obj)
        {
            String class_name = obj.GetType().Name;

            string[] id_names = new string[] { "ID", "id", "Id" };

            foreach (string id_name in id_names)
            {
                PropertyInfo property = obj.GetType().GetProperty(id_name);
                if (property != null)
                    return property.GetValue(obj, null).ToString();
            }

            // not found anything relevant
            throw new SystemException("Class " + class_name + " doesn't have any ID fields, please specify them explicitly");
        }

        private static String get_id_property(String id_name, Object obj)
        {
            PropertyInfo property = obj.GetType().GetProperty(id_name);
            if (property != null)
                return property.GetValue(obj, null).ToString();

            // not found anything relevant
            throw new SystemException("Class " + obj.GetType().Name + " doesn't have " + id_name + " property!");
        }
    }
}