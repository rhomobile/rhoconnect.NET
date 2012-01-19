using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using System.Reflection;
using System.Web.Script.Serialization;

namespace RhoconnectNET
{
    public class Helpers
    {
        public delegate bool rhoAuthHandler(ref String username, String password, Hashtable attrs);
        public static rhoAuthHandler _auth_handler;
//public static Func<ref String, String, Hashtable, bool> _auth_handler;

        public static bool rhoconnect_authenticate(ref String username, String password, Hashtable attrs)
        {
            if (_auth_handler != null)
            {
                return _auth_handler(ref username, password, attrs);
            }

            return true;
        }

        public static ActionResult serialize_result(object id)
        {
            return new ContentResult
            {
                Content = id.ToString(),
                ContentType = "text/plain"
            };
        }

        public static Object deserialize_json(String objJson, Type targetType)
        {
            return new JavaScriptSerializer().Deserialize(objJson, targetType);
        }

        public static void merge_changes(Object obj, Dictionary<string, object> changes)
        {
            foreach (KeyValuePair<string, object> kvp in changes)
            {
                PropertyInfo property = obj.GetType().GetProperty(kvp.Key);
                // skip unknown properties
                if (property == null)
                    continue;
                Object convValue = ChangeType(kvp.Value, property.PropertyType);
                property.SetValue(obj, convValue, null);
            }
        }
        
        public static object ChangeType(object value, Type conversionType)
        {
            // Note: This if block was taken from Convert.ChangeType as is, and is needed here since we're
            // checking properties on conversionType below.
            if (conversionType == null)
            {
                throw new ArgumentNullException("conversionType");
            } // end if

            // If it's not a nullable type, just pass through the parameters to Convert.ChangeType

            if (conversionType.IsGenericType &&
              conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                // It's a nullable type, so determine what the underlying type is
                if (value == null)
                {
                    return null;
                }

                // It's a nullable type, and not null, so that means it can be converted to its underlying type,
                // so overwrite the passed-in conversion type with this underlying type
                NullableConverter nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            } // end if

            // Now that we've guaranteed conversionType is something Convert.ChangeType can handle (i.e. not a
            // nullable type), pass the call on to Convert.ChangeType
            return Convert.ChangeType(value, conversionType);
        }
    }
}