using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Net;
using System.Text;
using System.IO;
using System.Collections;

namespace RhoconnectNET
{
    public class Client
    {
        private static String _endpoint_url;
        private static String _api_token;

        public static bool set_app_endpoint(String endpoint,
                                            String app_endpoint,
                                            String api_token,
                                            Func<String, String, Hashtable, bool> auth_handler)
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

        public static bool notify_on_create(String source_name, String partition,
                                            Hashtable objects)
        {
            Hashtable reqHash = new Hashtable();
            reqHash.Add("objects", objects); 
            return send_objects("push_objects", source_name, partition, reqHash);
        }

        public static bool notify_on_update(String source_name, String partition, Hashtable objects)
        {
            Hashtable reqHash = new Hashtable();
            reqHash.Add("objects", objects);
            return send_objects("push_objects", source_name, partition, reqHash);
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

    }
}