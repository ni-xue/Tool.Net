using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Tool.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Tool.Web.Builder;
using Tool.Web.Session;
using Tool.Utils;
using Tool.SqlCore;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Tool;
using System.Diagnostics;
using Tool.Utils.ActionDelegate;
using Tool.Sockets.WebHelper;
using Microsoft.AspNetCore.Routing;
using Tool.Sockets.NetFrame;
using Tool.Sockets.Kernels;
using System.Threading;
using System.Buffers;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Tool.Utils.Data;
using System.Collections;
using Microsoft.Data.SqlClient;

namespace WebTestApp
{
    //internal class DictionaryConverter : System.Text.Json.Serialization.JsonConverter
    //{
    //    public DictionaryConverter(string s) 
    //    {
    //        System.Text.Json.Serialization.


    //    }
    //    public override bool CanConvert(Type objectType)
    //    {
    //        return true;
    //    }
    //}

    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            AppSettings.GetSection("Test:a:b").Value = new string[] { "1", "2", "3", "7", "8", "9" }.ToJson();

            AppSettings.GetSection("Test:name").Value = new { a = 1, b = 2, c = "666666" }.ToJson();
            AppSettings.GetSection("Test:name").Value = new { a = 1, b = 2, c = "666666" }.ToJson();

            AppSettings.GetSection("Test:int").Value = "我和你";
            AppSettings.GetSection("Test:list:2:name").Value = "我";
            AppSettings.GetSection("Test:list:1:0").Value = "我";

            //services.AddSession();

            //var s = HttpHelpers.Timeout;

            //HttpHelpers.Timeout = 3000;

            ////var s1 = HttpHelpers.GetString("https://v1.hitokoto.cn/");

            ////var s2 = HttpHelpers.GetString("https://v1.hitokoto.cn/");

            ////var s3 = HttpHelpers.GetString("https://v1.hitokoto.cn/");

            ////var s4 = HttpHelpers.GetString("https://v1.hitokoto.cn/");

            //var s1 = HttpHelpers.PostString("https://sdk-tj.img4399.com/playtime/collect.html", "action=APP_DID_BECOME_ACTIVE&device={\"DEVICE_IDENTIFIER\":\"\",\"SCREEN_RESOLUTION\":\"2340*1036\",\"DEVICE_MODEL\":\"Redmi K20 Pro\",\"DEVICE_MODEL_VERSION\":\"11\",\"SYSTEM_VERSION\":\"11\",\"PLATFORM_TYPE\":\"Android\",\"SDK_VERSION\":\"2.37.0.214\",\"GAME_KEY\":\"40025\",\"GAME_VERSION\":\"12.1.1\",\"BID\":\"org.yjmobile.zmxy\",\"IMSI\":\"\",\"PHONE\":\"\",\"RUNTIME\":\"Origin\",\"CANAL_IDENTIFIER\":\"\",\"UDID\":\"1100gihU8AkKanE4wnDVX6dac\",\"DEBUG\":\"false\",\"NETWORK_TYPE\":\"WIFI\",\"SERVER_SERIAL\":\"0\",\"UID\":\"266873866\"}");

            //WebServer webServer = new();
            //webServer.StartAsync("127.0.0.1", 9999, false);

            //ClientFrame client = new(NetBufferSize.Default, true);
            //client.SetCompleted((a, b, c) =>
            //{
            //    Console.WriteLine("\nIP:{0} \t{1} \t{2}", a, b, c.ToString("yyyy/MM/dd HH:mm:ss:fffffff"));
            //    return ValueTask.CompletedTask;
            //});

            //client.ConnectAsync("127.0.0.1", 444).Wait();//120.79.58.17 
            //client.AddKeepAlive(5);

            //services.AddObject(client);

            services.AddRouting();

            services.AddMvc(o => o.EnableEndpointRouting = true);
            //services.AddAshx().AddHttpContext();

            services.AddAshx(o =>
            {
                o.EnableEndpointRouting = true;
                //o.IsAsync = true;
                //o.JsonOptions = new System.Text.Json.JsonSerializerOptions
                //{
                //    //IgnoreReadOnlyFields = true,
                //    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),
                //};

                //o.JsonOptions.Converters.Add(JsonConverterHelper.GetDateConverter());

                //o.JsonOptions.Converters.Add(JsonConverterHelper.GetDBNullConverter());
            }).AddHttpContext();

            services.AddDiySession(d =>
            {
                d.GetDiySession<Test.Class>();
                d.Cookie.Path = "/";
                //d.Cookie.SameSite = SameSiteMode.None;
                //d.Cookie.Secure = true;
                d.GetKey = async (s, v) =>
                {
                    return await Task.FromResult(v);
                };
                d.Sign = "666";
            });

            Test.Class1 class1 = new();
            class1.sd();

            //client.CopyEntity(class1,"b=>a1","");

            var str = """{ "result": { "code":0, "hehe": [0,5,10] } }""";

            var json = str.JsonVar();

            JsonVar code = json["result"]["hehe"][2];

            int _code = json["result"]["hehe"][2];

            //int _code = json["result"]["hehe"][2];

            Console.WriteLine();

            //var sda1 = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>("{ \"aa\":{\"a1\":\"aa11\",\"a2\":\"https:\\/\\/www.abc.com\\/asfasd8asfad\"},\"bb\":\"bbbbbbbbbasdfasdxxx\"}");

            //Dictionary<string, dynamic> sda = 
            //    ("{\"aa\":{\"a1\":\"aa11\",\"a2\":\"https:\\/\\/www.abc.com\\/asfasd8asfad\"},\"bb\":\"2021-04-28T01:01:55\"," +
            //    "\"cc\":-2147483648.234,\"dd\":[{\"aa\":{\"a1\":\"aa11\",\"a2\":\"https:\\/\\/www.abc.com\\/asfasd8asfad\"}}," +
            //    "{\"aa\":{\"a1\":\"aa11\",\"a2\":\"https:\\/\\/www.abc.com\\/asfasd8asfad\"}},\"1236971\", 12345 ]}").Json();

            //Console.WriteLine(sda["aa"]["a1"]);
            //Console.WriteLine(sda["aa"]["a2"]);
            //Console.WriteLine(sda["dd"][2]);


            //dynamic sda2 = ("[{\"aa\":{\"a1\":\"aa11\",\"a2\":\"https://www.abc.com/asfasd8asfad\"}}," +
            //    "{\"aa\":{\"a1\":\"aa11\",\"a2\":\"https://www.abc.com/asfasd8asfad\"}},\"1236971\", 12345 ]").JsonDynamic();

            //Console.WriteLine(sda2[0]["aa"]["a1"]);
            //Console.WriteLine(sda2[1]["aa"]["a2"]);
            //Console.WriteLine(sda2[2]);

            //var sas = "{\"title\": \"A JSON Schema for Swagger 2.0 API.\",   \"id\": \"http://swagger.io/v2/schema.json#\",   \"$schema\": \"http://json-schema.org/draft-04/schema#\",   \"type\": \"object\",   \"required\": [     \"swagger\",     \"info\",     \"paths\"   ],   \"additionalProperties\": false,   \"patternProperties\": {     \"^x-\": {       \"$ref\": \"#/definitions/vendorExtension\"     }   },   \"properties\": {     \"swagger\": {       \"type\": \"string\",       \"enum\": [         \"2.0\"       ],       \"description\": \"The Swagger version of this document.\"     },     \"info\": {       \"$ref\": \"#/definitions/info\"     },     \"host\": {       \"type\": \"string\",       \"pattern\": \"^[^{}/ :\\\\\\\\]+(?::\\\\d+)?$\",       \"description\": \"The host (name or ip) of the API. Example: 'swagger.io'\"     },     \"basePath\": {       \"type\": \"string\",       \"pattern\": \"^/\",       \"description\": \"The base path to the API. Example: '/api'.\"     },     \"schemes\": {       \"$ref\": \"#/definitions/schemesList\"     },     \"consumes\": {       \"description\": \"A list of MIME types accepted by the API.\",       \"allOf\": [         {           \"$ref\": \"#/definitions/mediaTypeList\"         }       ]     },     \"produces\": {       \"description\": \"A list of MIME types the API can produce.\",       \"allOf\": [         {           \"$ref\": \"#/definitions/mediaTypeList\"         }       ]     },     \"paths\": {       \"$ref\": \"#/definitions/paths\"     },     \"definitions\": {       \"$ref\": \"#/definitions/definitions\"     },     \"parameters\": {       \"$ref\": \"#/definitions/parameterDefinitions\"     },     \"responses\": {       \"$ref\": \"#/definitions/responseDefinitions\"     },     \"security\": {       \"$ref\": \"#/definitions/security\"     },     \"securityDefinitions\": {       \"$ref\": \"#/definitions/securityDefinitions\"     },     \"tags\": {       \"type\": \"array\",       \"items\": {         \"$ref\": \"#/definitions/tag\"       },       \"uniqueItems\": true     },     \"externalDocs\": {       \"$ref\": \"#/definitions/externalDocs\"     }   },   \"definitions\": {     \"info\": {       \"type\": \"object\",       \"description\": \"General information about the API.\",       \"required\": [         \"version\",         \"title\"       ],       \"additionalProperties\": false,       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       },       \"properties\": {         \"title\": {           \"type\": \"string\",           \"description\": \"A unique and precise title of the API.\"         },         \"version\": {           \"type\": \"string\",           \"description\": \"A semantic version number of the API.\"         },         \"description\": {           \"type\": \"string\",           \"description\": \"A longer description of the API. Should be different from the title.  GitHub Flavored Markdown is allowed.\"         },         \"termsOfService\": {           \"type\": \"string\",           \"description\": \"The terms of service for the API.\"         },         \"contact\": {           \"$ref\": \"#/definitions/contact\"         },         \"license\": {           \"$ref\": \"#/definitions/license\"         }       }     },     \"contact\": {       \"type\": \"object\",       \"description\": \"Contact information for the owners of the API.\",       \"additionalProperties\": false,       \"properties\": {         \"name\": {           \"type\": \"string\",           \"description\": \"The identifying name of the contact person/organization.\"         },         \"url\": {           \"type\": \"string\",           \"description\": \"The URL pointing to the contact information.\",           \"format\": \"uri\"         },         \"email\": {           \"type\": \"string\",           \"description\": \"The email address of the contact person/organization.\",           \"format\": \"email\"         }       },       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       }     },     \"license\": {       \"type\": \"object\",       \"required\": [         \"name\"       ],       \"additionalProperties\": false,       \"properties\": {         \"name\": {           \"type\": \"string\",           \"description\": \"The name of the license type. It's encouraged to use an OSI compatible license.\"         },         \"url\": {           \"type\": \"string\",           \"description\": \"The URL pointing to the license.\",           \"format\": \"uri\"         }       },       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       }     },     \"paths\": {       \"type\": \"object\",       \"description\": \"Relative paths to the individual endpoints. They must be relative to the 'basePath'.\",       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         },         \"^/\": {           \"$ref\": \"#/definitions/pathItem\"         }       },       \"additionalProperties\": false     },     \"definitions\": {       \"type\": \"object\",       \"additionalProperties\": {         \"$ref\": \"#/definitions/schema\"       },       \"description\": \"One or more JSON objects describing the schemas being consumed and produced by the API.\"     },     \"parameterDefinitions\": {       \"type\": \"object\",       \"additionalProperties\": {         \"$ref\": \"#/definitions/parameter\"       },       \"description\": \"One or more JSON representations for parameters\"     },     \"responseDefinitions\": {       \"type\": \"object\",       \"additionalProperties\": {         \"$ref\": \"#/definitions/response\"       },       \"description\": \"One or more JSON representations for responses\"     },     \"externalDocs\": {       \"type\": \"object\",       \"additionalProperties\": false,       \"description\": \"information about external documentation\",       \"required\": [         \"url\"       ],       \"properties\": {         \"description\": {           \"type\": \"string\"         },         \"url\": {           \"type\": \"string\",           \"format\": \"uri\"         }       },       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       }     },     \"examples\": {       \"type\": \"object\",       \"additionalProperties\": true     },     \"mimeType\": {       \"type\": \"string\",       \"description\": \"The MIME type of the HTTP message.\"     },     \"operation\": {       \"type\": \"object\",       \"required\": [         \"responses\"       ],       \"additionalProperties\": false,       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       },       \"properties\": {         \"tags\": {           \"type\": \"array\",           \"items\": {             \"type\": \"string\"           },           \"uniqueItems\": true         },         \"summary\": {           \"type\": \"string\",           \"description\": \"A brief summary of the operation.\"         },         \"description\": {           \"type\": \"string\",           \"description\": \"A longer description of the operation, GitHub Flavored Markdown is allowed.\"         },         \"externalDocs\": {           \"$ref\": \"#/definitions/externalDocs\"         },         \"operationId\": {           \"type\": \"string\",           \"description\": \"A unique identifier of the operation.\"         },         \"produces\": {           \"description\": \"A list of MIME types the API can produce.\",           \"allOf\": [             {               \"$ref\": \"#/definitions/mediaTypeList\"             }           ]         },         \"consumes\": {           \"description\": \"A list of MIME types the API can consume.\",           \"allOf\": [             {               \"$ref\": \"#/definitions/mediaTypeList\"             }           ]         },         \"parameters\": {           \"$ref\": \"#/definitions/parametersList\"         },         \"responses\": {           \"$ref\": \"#/definitions/responses\"         },         \"schemes\": {           \"$ref\": \"#/definitions/schemesList\"         },         \"deprecated\": {           \"type\": \"boolean\",           \"default\": false         },         \"security\": {           \"$ref\": \"#/definitions/security\"         }       }     },     \"pathItem\": {       \"type\": \"object\",       \"additionalProperties\": false,       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       },       \"properties\": {         \"$ref\": {           \"type\": \"string\"         },         \"get\": {           \"$ref\": \"#/definitions/operation\"         },         \"put\": {           \"$ref\": \"#/definitions/operation\"         },         \"post\": {           \"$ref\": \"#/definitions/operation\"         },         \"delete\": {           \"$ref\": \"#/definitions/operation\"         },         \"options\": {           \"$ref\": \"#/definitions/operation\"         },         \"head\": {           \"$ref\": \"#/definitions/operation\"         },         \"patch\": {           \"$ref\": \"#/definitions/operation\"         },         \"parameters\": {           \"$ref\": \"#/definitions/parametersList\"         }       }     },     \"responses\": {       \"type\": \"object\",       \"description\": \"Response objects names can either be any valid HTTP status code or 'default'.\",       \"minProperties\": 1,       \"additionalProperties\": false,       \"patternProperties\": {         \"^([0-9]{3})$|^(default)$\": {           \"$ref\": \"#/definitions/responseValue\"         },         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       },       \"not\": {         \"type\": \"object\",         \"additionalProperties\": false,         \"patternProperties\": {           \"^x-\": {             \"$ref\": \"#/definitions/vendorExtension\"           }         }       }     },     \"responseValue\": {       \"oneOf\": [         {           \"$ref\": \"#/definitions/response\"         },         {           \"$ref\": \"#/definitions/jsonReference\"         }       ]     },     \"response\": {       \"type\": \"object\",       \"required\": [         \"description\"       ],       \"properties\": {         \"description\": {           \"type\": \"string\"         },         \"schema\": {           \"oneOf\": [             {               \"$ref\": \"#/definitions/schema\"             },             {               \"$ref\": \"#/definitions/fileSchema\"             }           ]         },         \"headers\": {           \"$ref\": \"#/definitions/headers\"         },         \"examples\": {           \"$ref\": \"#/definitions/examples\"         }       },       \"additionalProperties\": false,       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       }     },     \"headers\": {       \"type\": \"object\",       \"additionalProperties\": {         \"$ref\": \"#/definitions/header\"       }     },     \"header\": {       \"type\": \"object\",       \"additionalProperties\": false,       \"required\": [         \"type\"       ],       \"properties\": {         \"type\": {           \"type\": \"string\",           \"enum\": [             \"string\",             \"number\",             \"integer\",             \"boolean\",             \"array\"           ]         },         \"format\": {           \"type\": \"string\"         },         \"items\": {           \"$ref\": \"#/definitions/primitivesItems\"         },         \"collectionFormat\": {           \"$ref\": \"#/definitions/collectionFormat\"         },         \"default\": {           \"$ref\": \"#/definitions/default\"         },         \"maximum\": {           \"$ref\": \"#/definitions/maximum\"         },         \"exclusiveMaximum\": {           \"$ref\": \"#/definitions/exclusiveMaximum\"         },         \"minimum\": {           \"$ref\": \"#/definitions/minimum\"         },         \"exclusiveMinimum\": {           \"$ref\": \"#/definitions/exclusiveMinimum\"         },         \"maxLength\": {           \"$ref\": \"#/definitions/maxLength\"         },         \"minLength\": {           \"$ref\": \"#/definitions/minLength\"         },         \"pattern\": {           \"$ref\": \"#/definitions/pattern\"         },         \"maxItems\": {           \"$ref\": \"#/definitions/maxItems\"         },         \"minItems\": {           \"$ref\": \"#/definitions/minItems\"         },         \"uniqueItems\": {           \"$ref\": \"#/definitions/uniqueItems\"         },         \"enum\": {           \"$ref\": \"#/definitions/enum\"         },         \"multipleOf\": {           \"$ref\": \"#/definitions/multipleOf\"         },         \"description\": {           \"type\": \"string\"         }       },       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       }     },     \"vendorExtension\": {       \"description\": \"Any property starting with x- is valid.\",       \"additionalProperties\": true,       \"additionalItems\": true     },     \"bodyParameter\": {       \"type\": \"object\",       \"required\": [         \"name\",         \"in\",         \"schema\"       ],       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       },       \"properties\": {         \"description\": {           \"type\": \"string\",           \"description\": \"A brief description of the parameter. This could contain examples of use.  GitHub Flavored Markdown is allowed.\"         },         \"name\": {           \"type\": \"string\",           \"description\": \"The name of the parameter.\"         },         \"in\": {           \"type\": \"string\",           \"description\": \"Determines the location of the parameter.\",           \"enum\": [             \"body\"           ]         },         \"required\": {           \"type\": \"boolean\",           \"description\": \"Determines whether or not this parameter is required or optional.\",           \"default\": false         },         \"schema\": {           \"$ref\": \"#/definitions/schema\"         }       },       \"additionalProperties\": false     },     \"headerParameterSubSchema\": {       \"additionalProperties\": false,       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       },       \"properties\": {         \"required\": {           \"type\": \"boolean\",           \"description\": \"Determines whether or not this parameter is required or optional.\",           \"default\": false         },         \"in\": {           \"type\": \"string\",           \"description\": \"Determines the location of the parameter.\",           \"enum\": [             \"header\"           ]         },         \"description\": {           \"type\": \"string\",           \"description\": \"A brief description of the parameter. This could contain examples of use.  GitHub Flavored Markdown is allowed.\"         },         \"name\": {           \"type\": \"string\",           \"description\": \"The name of the parameter.\"         },         \"type\": {           \"type\": \"string\",           \"enum\": [             \"string\",             \"number\",             \"boolean\",             \"integer\",             \"array\"           ]         },         \"format\": {           \"type\": \"string\"         },         \"items\": {           \"$ref\": \"#/definitions/primitivesItems\"         },         \"collectionFormat\": {           \"$ref\": \"#/definitions/collectionFormat\"         },         \"default\": {           \"$ref\": \"#/definitions/default\"         },         \"maximum\": {           \"$ref\": \"#/definitions/maximum\"         },         \"exclusiveMaximum\": {           \"$ref\": \"#/definitions/exclusiveMaximum\"         },         \"minimum\": {           \"$ref\": \"#/definitions/minimum\"         },         \"exclusiveMinimum\": {           \"$ref\": \"#/definitions/exclusiveMinimum\"         },         \"maxLength\": {           \"$ref\": \"#/definitions/maxLength\"         },         \"minLength\": {           \"$ref\": \"#/definitions/minLength\"         },         \"pattern\": {           \"$ref\": \"#/definitions/pattern\"         },         \"maxItems\": {           \"$ref\": \"#/definitions/maxItems\"         },         \"minItems\": {           \"$ref\": \"#/definitions/minItems\"         },         \"uniqueItems\": {           \"$ref\": \"#/definitions/uniqueItems\"         },         \"enum\": {           \"$ref\": \"#/definitions/enum\"         },         \"multipleOf\": {           \"$ref\": \"#/definitions/multipleOf\"         }       }     },     \"queryParameterSubSchema\": {       \"additionalProperties\": false,       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       },       \"properties\": {         \"required\": {           \"type\": \"boolean\",           \"description\": \"Determines whether or not this parameter is required or optional.\",           \"default\": false         },         \"in\": {           \"type\": \"string\",           \"description\": \"Determines the location of the parameter.\",           \"enum\": [             \"query\"           ]         },         \"description\": {           \"type\": \"string\",           \"description\": \"A brief description of the parameter. This could contain examples of use.  GitHub Flavored Markdown is allowed.\"         },         \"name\": {           \"type\": \"string\",           \"description\": \"The name of the parameter.\"         },         \"allowEmptyValue\": {           \"type\": \"boolean\",           \"default\": false,           \"description\": \"allows sending a parameter by name only or with an empty value.\"         },         \"type\": {           \"type\": \"string\",           \"enum\": [             \"string\",             \"number\",             \"boolean\",             \"integer\",             \"array\"           ]         },         \"format\": {           \"type\": \"string\"         },         \"items\": {           \"$ref\": \"#/definitions/primitivesItems\"         },         \"collectionFormat\": {           \"$ref\": \"#/definitions/collectionFormatWithMulti\"         },         \"default\": {           \"$ref\": \"#/definitions/default\"         },         \"maximum\": {           \"$ref\": \"#/definitions/maximum\"         },         \"exclusiveMaximum\": {           \"$ref\": \"#/definitions/exclusiveMaximum\"         },         \"minimum\": {           \"$ref\": \"#/definitions/minimum\"         },         \"exclusiveMinimum\": {           \"$ref\": \"#/definitions/exclusiveMinimum\"         },         \"maxLength\": {           \"$ref\": \"#/definitions/maxLength\"         },         \"minLength\": {           \"$ref\": \"#/definitions/minLength\"         },         \"pattern\": {           \"$ref\": \"#/definitions/pattern\"         },         \"maxItems\": {           \"$ref\": \"#/definitions/maxItems\"         },         \"minItems\": {           \"$ref\": \"#/definitions/minItems\"         },         \"uniqueItems\": {           \"$ref\": \"#/definitions/uniqueItems\"         },         \"enum\": {           \"$ref\": \"#/definitions/enum\"         },         \"multipleOf\": {           \"$ref\": \"#/definitions/multipleOf\"         }       }     },     \"formDataParameterSubSchema\": {       \"additionalProperties\": false,       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       },       \"properties\": {         \"required\": {           \"type\": \"boolean\",           \"description\": \"Determines whether or not this parameter is required or optional.\",           \"default\": false         },         \"in\": {           \"type\": \"string\",           \"description\": \"Determines the location of the parameter.\",           \"enum\": [             \"formData\"           ]         },         \"description\": {           \"type\": \"string\",           \"description\": \"A brief description of the parameter. This could contain examples of use.  GitHub Flavored Markdown is allowed.\"         },         \"name\": {           \"type\": \"string\",           \"description\": \"The name of the parameter.\"         },         \"allowEmptyValue\": {           \"type\": \"boolean\",           \"default\": false,           \"description\": \"allows sending a parameter by name only or with an empty value.\"         },         \"type\": {           \"type\": \"string\",           \"enum\": [             \"string\",             \"number\",             \"boolean\",             \"integer\",             \"array\",             \"file\"           ]         },         \"format\": {           \"type\": \"string\"         },         \"items\": {           \"$ref\": \"#/definitions/primitivesItems\"         },         \"collectionFormat\": {           \"$ref\": \"#/definitions/collectionFormatWithMulti\"         },         \"default\": {           \"$ref\": \"#/definitions/default\"         },         \"maximum\": {           \"$ref\": \"#/definitions/maximum\"         },         \"exclusiveMaximum\": {           \"$ref\": \"#/definitions/exclusiveMaximum\"         },         \"minimum\": {           \"$ref\": \"#/definitions/minimum\"         },         \"exclusiveMinimum\": {           \"$ref\": \"#/definitions/exclusiveMinimum\"         },         \"maxLength\": {           \"$ref\": \"#/definitions/maxLength\"         },         \"minLength\": {           \"$ref\": \"#/definitions/minLength\"         },         \"pattern\": {           \"$ref\": \"#/definitions/pattern\"         },         \"maxItems\": {           \"$ref\": \"#/definitions/maxItems\"         },         \"minItems\": {           \"$ref\": \"#/definitions/minItems\"         },         \"uniqueItems\": {           \"$ref\": \"#/definitions/uniqueItems\"         },         \"enum\": {           \"$ref\": \"#/definitions/enum\"         },         \"multipleOf\": {           \"$ref\": \"#/definitions/multipleOf\"         }       }     },     \"pathParameterSubSchema\": {       \"additionalProperties\": false,       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       },       \"required\": [         \"required\"       ],       \"properties\": {         \"required\": {           \"type\": \"boolean\",           \"enum\": [             true           ],           \"description\": \"Determines whether or not this parameter is required or optional.\"         },         \"in\": {           \"type\": \"string\",           \"description\": \"Determines the location of the parameter.\",           \"enum\": [             \"path\"           ]         },         \"description\": {           \"type\": \"string\",           \"description\": \"A brief description of the parameter. This could contain examples of use.  GitHub Flavored Markdown is allowed.\"         },         \"name\": {           \"type\": \"string\",           \"description\": \"The name of the parameter.\"         },         \"type\": {           \"type\": \"string\",           \"enum\": [             \"string\",             \"number\",             \"boolean\",             \"integer\",             \"array\"           ]         },         \"format\": {           \"type\": \"string\"         },         \"items\": {           \"$ref\": \"#/definitions/primitivesItems\"         },         \"collectionFormat\": {           \"$ref\": \"#/definitions/collectionFormat\"         },         \"default\": {           \"$ref\": \"#/definitions/default\"         },         \"maximum\": {           \"$ref\": \"#/definitions/maximum\"         },         \"exclusiveMaximum\": {           \"$ref\": \"#/definitions/exclusiveMaximum\"         },         \"minimum\": {           \"$ref\": \"#/definitions/minimum\"         },         \"exclusiveMinimum\": {           \"$ref\": \"#/definitions/exclusiveMinimum\"         },         \"maxLength\": {           \"$ref\": \"#/definitions/maxLength\"         },         \"minLength\": {           \"$ref\": \"#/definitions/minLength\"         },         \"pattern\": {           \"$ref\": \"#/definitions/pattern\"         },         \"maxItems\": {           \"$ref\": \"#/definitions/maxItems\"         },         \"minItems\": {           \"$ref\": \"#/definitions/minItems\"         },         \"uniqueItems\": {           \"$ref\": \"#/definitions/uniqueItems\"         },         \"enum\": {           \"$ref\": \"#/definitions/enum\"         },         \"multipleOf\": {           \"$ref\": \"#/definitions/multipleOf\"         }       }     },     \"nonBodyParameter\": {       \"type\": \"object\",       \"required\": [         \"name\",         \"in\",         \"type\"       ],       \"oneOf\": [         {           \"$ref\": \"#/definitions/headerParameterSubSchema\"         },         {           \"$ref\": \"#/definitions/formDataParameterSubSchema\"         },         {           \"$ref\": \"#/definitions/queryParameterSubSchema\"         },         {           \"$ref\": \"#/definitions/pathParameterSubSchema\"         }       ]     },     \"parameter\": {       \"oneOf\": [         {           \"$ref\": \"#/definitions/bodyParameter\"         },         {           \"$ref\": \"#/definitions/nonBodyParameter\"         }       ]     },     \"schema\": {       \"type\": \"object\",       \"description\": \"A deterministic version of a JSON Schema object.\",       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       },       \"properties\": {         \"$ref\": {           \"type\": \"string\"         },         \"format\": {           \"type\": \"string\"         },         \"title\": {           \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/title\"         },         \"description\": {           \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/description\"         },         \"default\": {           \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/default\"         },         \"multipleOf\": {           \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/multipleOf\"         },         \"maximum\": {           \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/maximum\"         },         \"exclusiveMaximum\": {           \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/exclusiveMaximum\"         },         \"minimum\": {           \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/minimum\"         },         \"exclusiveMinimum\": {           \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/exclusiveMinimum\"         },         \"maxLength\": {           \"$ref\": \"http://json-schema.org/draft-04/schema#/definitions/positiveInteger\"         },         \"minLength\": {           \"$ref\": \"http://json-schema.org/draft-04/schema#/definitions/positiveIntegerDefault0\"         },         \"pattern\": {           \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/pattern\"         },         \"maxItems\": {           \"$ref\": \"http://json-schema.org/draft-04/schema#/definitions/positiveInteger\"         },         \"minItems\": {           \"$ref\": \"http://json-schema.org/draft-04/schema#/definitions/positiveIntegerDefault0\"         },         \"uniqueItems\": {           \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/uniqueItems\"         },         \"maxProperties\": {           \"$ref\": \"http://json-schema.org/draft-04/schema#/definitions/positiveInteger\"         },         \"minProperties\": {           \"$ref\": \"http://json-schema.org/draft-04/schema#/definitions/positiveIntegerDefault0\"         },         \"required\": {           \"$ref\": \"http://json-schema.org/draft-04/schema#/definitions/stringArray\"         },         \"enum\": {           \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/enum\"         },         \"additionalProperties\": {           \"anyOf\": [             {               \"$ref\": \"#/definitions/schema\"             },             {               \"type\": \"boolean\"             }           ],           \"default\": {}         },         \"type\": {           \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/type\"         },         \"items\": {           \"anyOf\": [             {               \"$ref\": \"#/definitions/schema\"             },             {               \"type\": \"array\",               \"minItems\": 1,               \"items\": {                 \"$ref\": \"#/definitions/schema\"               }             }           ],           \"default\": {}         },         \"allOf\": {           \"type\": \"array\",           \"minItems\": 1,           \"items\": {             \"$ref\": \"#/definitions/schema\"           }         },         \"properties\": {           \"type\": \"object\",           \"additionalProperties\": {             \"$ref\": \"#/definitions/schema\"           },           \"default\": {}         },         \"discriminator\": {           \"type\": \"string\"         },         \"readOnly\": {           \"type\": \"boolean\",           \"default\": false         },         \"xml\": {           \"$ref\": \"#/definitions/xml\"         },         \"externalDocs\": {           \"$ref\": \"#/definitions/externalDocs\"         },         \"example\": {}       },       \"additionalProperties\": false     },     \"fileSchema\": {       \"type\": \"object\",       \"description\": \"A deterministic version of a JSON Schema object.\",       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       },       \"required\": [         \"type\"       ],       \"properties\": {         \"format\": {           \"type\": \"string\"         },         \"title\": {           \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/title\"         },         \"description\": {           \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/description\"         },         \"default\": {           \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/default\"         },         \"required\": {           \"$ref\": \"http://json-schema.org/draft-04/schema#/definitions/stringArray\"         },         \"type\": {           \"type\": \"string\",           \"enum\": [             \"file\"           ]         },         \"readOnly\": {           \"type\": \"boolean\",           \"default\": false         },         \"externalDocs\": {           \"$ref\": \"#/definitions/externalDocs\"         },         \"example\": {}       },       \"additionalProperties\": false     },     \"primitivesItems\": {       \"type\": \"object\",       \"additionalProperties\": false,       \"properties\": {         \"type\": {           \"type\": \"string\",           \"enum\": [             \"string\",             \"number\",             \"integer\",             \"boolean\",             \"array\"           ]         },         \"format\": {           \"type\": \"string\"         },         \"items\": {           \"$ref\": \"#/definitions/primitivesItems\"         },         \"collectionFormat\": {           \"$ref\": \"#/definitions/collectionFormat\"         },         \"default\": {           \"$ref\": \"#/definitions/default\"         },         \"maximum\": {           \"$ref\": \"#/definitions/maximum\"         },         \"exclusiveMaximum\": {           \"$ref\": \"#/definitions/exclusiveMaximum\"         },         \"minimum\": {           \"$ref\": \"#/definitions/minimum\"         },         \"exclusiveMinimum\": {           \"$ref\": \"#/definitions/exclusiveMinimum\"         },         \"maxLength\": {           \"$ref\": \"#/definitions/maxLength\"         },         \"minLength\": {           \"$ref\": \"#/definitions/minLength\"         },         \"pattern\": {           \"$ref\": \"#/definitions/pattern\"         },         \"maxItems\": {           \"$ref\": \"#/definitions/maxItems\"         },         \"minItems\": {           \"$ref\": \"#/definitions/minItems\"         },         \"uniqueItems\": {           \"$ref\": \"#/definitions/uniqueItems\"         },         \"enum\": {           \"$ref\": \"#/definitions/enum\"         },         \"multipleOf\": {           \"$ref\": \"#/definitions/multipleOf\"         }       },       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       }     },     \"security\": {       \"type\": \"array\",       \"items\": {         \"$ref\": \"#/definitions/securityRequirement\"       },       \"uniqueItems\": true     },     \"securityRequirement\": {       \"type\": \"object\",       \"additionalProperties\": {         \"type\": \"array\",         \"items\": {           \"type\": \"string\"         },         \"uniqueItems\": true       }     },     \"xml\": {       \"type\": \"object\",       \"additionalProperties\": false,       \"properties\": {         \"name\": {           \"type\": \"string\"         },         \"namespace\": {           \"type\": \"string\"         },         \"prefix\": {           \"type\": \"string\"         },         \"attribute\": {           \"type\": \"boolean\",           \"default\": false         },         \"wrapped\": {           \"type\": \"boolean\",           \"default\": false         }       },       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       }     },     \"tag\": {       \"type\": \"object\",       \"additionalProperties\": false,       \"required\": [         \"name\"       ],       \"properties\": {         \"name\": {           \"type\": \"string\"         },         \"description\": {           \"type\": \"string\"         },         \"externalDocs\": {           \"$ref\": \"#/definitions/externalDocs\"         }       },       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       }     },     \"securityDefinitions\": {       \"type\": \"object\",       \"additionalProperties\": {         \"oneOf\": [           {             \"$ref\": \"#/definitions/basicAuthenticationSecurity\"           },           {             \"$ref\": \"#/definitions/apiKeySecurity\"           },           {             \"$ref\": \"#/definitions/oauth2ImplicitSecurity\"           },           {             \"$ref\": \"#/definitions/oauth2PasswordSecurity\"           },           {             \"$ref\": \"#/definitions/oauth2ApplicationSecurity\"           },           {             \"$ref\": \"#/definitions/oauth2AccessCodeSecurity\"           }         ]       }     },     \"basicAuthenticationSecurity\": {       \"type\": \"object\",       \"additionalProperties\": false,       \"required\": [         \"type\"       ],       \"properties\": {         \"type\": {           \"type\": \"string\",           \"enum\": [             \"basic\"           ]         },         \"description\": {           \"type\": \"string\"         }       },       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       }     },     \"apiKeySecurity\": {       \"type\": \"object\",       \"additionalProperties\": false,       \"required\": [         \"type\",         \"name\",         \"in\"       ],       \"properties\": {         \"type\": {           \"type\": \"string\",           \"enum\": [             \"apiKey\"           ]         },         \"name\": {           \"type\": \"string\"         },         \"in\": {           \"type\": \"string\",           \"enum\": [             \"header\",             \"query\"           ]         },         \"description\": {           \"type\": \"string\"         }       },       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       }     },     \"oauth2ImplicitSecurity\": {       \"type\": \"object\",       \"additionalProperties\": false,       \"required\": [         \"type\",         \"flow\",         \"authorizationUrl\"       ],       \"properties\": {         \"type\": {           \"type\": \"string\",           \"enum\": [             \"oauth2\"           ]         },         \"flow\": {           \"type\": \"string\",           \"enum\": [             \"implicit\"           ]         },         \"scopes\": {           \"$ref\": \"#/definitions/oauth2Scopes\"         },         \"authorizationUrl\": {           \"type\": \"string\",           \"format\": \"uri\"         },         \"description\": {           \"type\": \"string\"         }       },       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       }     },     \"oauth2PasswordSecurity\": {       \"type\": \"object\",       \"additionalProperties\": false,       \"required\": [         \"type\",         \"flow\",         \"tokenUrl\"       ],       \"properties\": {         \"type\": {           \"type\": \"string\",           \"enum\": [             \"oauth2\"           ]         },         \"flow\": {           \"type\": \"string\",           \"enum\": [             \"password\"           ]         },         \"scopes\": {           \"$ref\": \"#/definitions/oauth2Scopes\"         },         \"tokenUrl\": {           \"type\": \"string\",           \"format\": \"uri\"         },         \"description\": {           \"type\": \"string\"         }       },       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       }     },     \"oauth2ApplicationSecurity\": {       \"type\": \"object\",       \"additionalProperties\": false,       \"required\": [         \"type\",         \"flow\",         \"tokenUrl\"       ],       \"properties\": {         \"type\": {           \"type\": \"string\",           \"enum\": [             \"oauth2\"           ]         },         \"flow\": {           \"type\": \"string\",           \"enum\": [             \"application\"           ]         },         \"scopes\": {           \"$ref\": \"#/definitions/oauth2Scopes\"         },         \"tokenUrl\": {           \"type\": \"string\",           \"format\": \"uri\"         },         \"description\": {           \"type\": \"string\"         }       },       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       }     },     \"oauth2AccessCodeSecurity\": {       \"type\": \"object\",       \"additionalProperties\": false,       \"required\": [         \"type\",         \"flow\",         \"authorizationUrl\",         \"tokenUrl\"       ],       \"properties\": {         \"type\": {           \"type\": \"string\",           \"enum\": [             \"oauth2\"           ]         },         \"flow\": {           \"type\": \"string\",           \"enum\": [             \"accessCode\"           ]         },         \"scopes\": {           \"$ref\": \"#/definitions/oauth2Scopes\"         },         \"authorizationUrl\": {           \"type\": \"string\",           \"format\": \"uri\"         },         \"tokenUrl\": {           \"type\": \"string\",           \"format\": \"uri\"         },         \"description\": {           \"type\": \"string\"         }       },       \"patternProperties\": {         \"^x-\": {           \"$ref\": \"#/definitions/vendorExtension\"         }       }     },     \"oauth2Scopes\": {       \"type\": \"object\",       \"additionalProperties\": {         \"type\": \"string\"       }     },     \"mediaTypeList\": {       \"type\": \"array\",       \"items\": {         \"$ref\": \"#/definitions/mimeType\"       },       \"uniqueItems\": true     },     \"parametersList\": {       \"type\": \"array\",       \"description\": \"The parameters needed to send a valid API call.\",       \"additionalItems\": false,       \"items\": {         \"oneOf\": [           {             \"$ref\": \"#/definitions/parameter\"           },           {             \"$ref\": \"#/definitions/jsonReference\"           }         ]       },       \"uniqueItems\": true     },     \"schemesList\": {       \"type\": \"array\",       \"description\": \"The transfer protocol of the API.\",       \"items\": {         \"type\": \"string\",         \"enum\": [           \"http\",           \"https\",           \"ws\",           \"wss\"         ]       },       \"uniqueItems\": true     },     \"collectionFormat\": {       \"type\": \"string\",       \"enum\": [         \"csv\",         \"ssv\",         \"tsv\",         \"pipes\"       ],       \"default\": \"csv\"     },     \"collectionFormatWithMulti\": {       \"type\": \"string\",       \"enum\": [         \"csv\",         \"ssv\",         \"tsv\",         \"pipes\",         \"multi\"       ],       \"default\": \"csv\"     },     \"title\": {       \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/title\"     },     \"description\": {       \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/description\"     },     \"default\": {       \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/default\"     },     \"multipleOf\": {       \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/multipleOf\"     },     \"maximum\": {       \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/maximum\"     },     \"exclusiveMaximum\": {       \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/exclusiveMaximum\"     },     \"minimum\": {       \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/minimum\"     },     \"exclusiveMinimum\": {       \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/exclusiveMinimum\"     },     \"maxLength\": {       \"$ref\": \"http://json-schema.org/draft-04/schema#/definitions/positiveInteger\"     },     \"minLength\": {       \"$ref\": \"http://json-schema.org/draft-04/schema#/definitions/positiveIntegerDefault0\"     },     \"pattern\": {       \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/pattern\"     },     \"maxItems\": {       \"$ref\": \"http://json-schema.org/draft-04/schema#/definitions/positiveInteger\"     },     \"minItems\": {       \"$ref\": \"http://json-schema.org/draft-04/schema#/definitions/positiveIntegerDefault0\"     },     \"uniqueItems\": {       \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/uniqueItems\"     },     \"enum\": {       \"$ref\": \"http://json-schema.org/draft-04/schema#/properties/enum\"     },     \"jsonReference\": {       \"type\": \"object\",       \"required\": [         \"$ref\"       ],       \"additionalProperties\": false,       \"properties\": {         \"$ref\": {           \"type\": \"string\"         }       },         \"enum\": [         \"pipes\",         \"multi\",           { \"s\": 126311111111111111111111111111111111111111111111111 },           123456       ]     }   } }".Json();
            //sda[]

            //VerificCodeHelper.BrushNames.Remove("DarkCyan");

            //var t1 = Task.Run(() =>
            //{
            //    for (int i = 0; i < 10000; i++)
            //    {
            //        //string code = VerificCodeHelper.GetRandomCodeV2(out int a);
            //        VerificCodeHelper.GetVCode("1234");
            //        //Log.Debug(VerificCodeHelper.GetVCode(code).ToBase64(), "1");
            //    }
            //});

            //var t2 = Task.Run(() =>
            //{
            //    for (int i = 0; i < 10000; i++)
            //    {
            //        //string code = VerificCodeHelper.GetRandomCodeV2(out int a);
            //        VerificCodeHelper.GetVCode("1234");
            //        //Log.Debug(VerificCodeHelper.GetVCode(code).ToBase64(), "2");
            //    }
            //});

            //Task.WaitAll(t1, t2);

            services.SetFormOptions(optins =>
            {
                optins.MultipartBodyLengthLimit = 60000000;
            });

            //Tool.Sockets.TcpFrame.ServerFrame server = new(9);
            //server.StartAsync(9999);

            //Tool.Sockets.TcpFrame.ClientFrame client = new(Tool.Sockets.SupportCode.TcpBufferSize.Default, 9, true);
            //client.ConnectAsync(9999);
            //client.AddKeepAlive(5);

            //Tool.Utils.ThreadQueue.WaitAction wait = new((a)=> 
            //{
            //    System.Threading.Thread.Sleep(3500);
            //}, 1);

            //for (int i = 0; i < 10; i++)
            //{
            //    Tool.Utils.ThreadQueue.ActionQueue.Add(wait);
            //}

            //Tool.Sockets.SupportCode.KeepAlive keep = new(1, ()=> 
            //{
            //    Console.WriteLine("000_1");

            //    //wait.Wait(out _);

            //    if (Tool.Utils.ThreadQueue.ActionLock.Start())
            //    {
            //        Console.WriteLine("111_1");
            //        Tool.Utils.ThreadQueue.ActionLock.End();
            //    }
            //});

            //Tool.Sockets.SupportCode.KeepAlive keep1 = new(1, () =>
            //{
            //    Console.WriteLine("000_2");

            //    //wait.Wait(out _);

            //    if (Tool.Utils.ThreadQueue.ActionLock.Start())
            //    {
            //        Console.WriteLine("111_2");
            //        //Tool.Utils.ThreadQueue.ActionLock.End();
            //    }
            //});

            //Tool.Utils.ThreadQueue.ActionQueue.ContinueWith += ActionQueue_ContinueWith;

            //wait.Run();

            string sqlstr = "user id=hkr1985sql123qwe;password=123qwe!@#QWEHello2019_c30110cb85e5a4e36ee9240081341132;initial catalog=HKR51;data source=127.0.0.1;TrustServerCertificate=true";
            services.AddObject(new DbHelper(sqlstr, DbProviderType.SqlServer1, new SqlServerProvider()));

            //services.AddObject(new DbHelper("data source=47.94.109.199;database=liquortribe;user id=liquortribe;password=NjCHBrzhrWpJZr8a;pooling=true;charset=utf8;", DbProviderType.MySql, new MySqlProvider()));
        }

        //private void ActionQueue_ContinueWith(Tool.Utils.ThreadQueue.WaitAction obj)
        //{
        //    //obj.Dispose();
        //    Console.WriteLine("完成");
        //}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)//, ClientFrame client
        {
            //HttpContextExtension.Current.//env.IsDevelopment

            //var s = ActionHelper<Test.Property>.GetActionMethodHelper(MethodFlags.Private);

            //s.ActionMethods[0].Action.Invoke()

            //var s = ActionHelper<ApplicationBuilder>.GetActionMethodHelper(MethodFlags.Private);

            EnumEventQueue.OnInterceptor(EnClient.SendMsg, true);
            EnumEventQueue.OnInterceptor(EnClient.Receive, true);

            DbHelper dbHelper = app.GetObject<DbHelper>();
            dbHelper.CommandTimeout = 1800;
            dbHelper.SetLogger(loggerFactory.CreateLogger("sql"));

            var dic = dbHelper.SelectDictionaryAsync("SELECT TOP (10000) * FROM [HKR51].[dbo].[XGDZ_CheckSub];").Result;

            var array = dbHelper.SelectArrayAsync("SELECT TOP (10000) * FROM [HKR51].[dbo].[XGDZ_CheckSub]  WHERE SerNo=@SerID", new { SerID  = 1 }).Result;

            string str0 = array.ToJson();
            string str1 = dic.ToJson();

            DataTable dt = new();
            dt.CloneArray(array);

            DataTable dt1 = new();
            var varjson0 = str0.JsonVar();

            var varjson1 = str1.JsonVar();

            if (varjson0.TryGet(out JsonVar varobj0, 6, 4))
            {
                Console.WriteLine(varobj0.ToString());
            }

            if (varjson1.TryGet(out JsonVar varobj1, 5, "FromNo"))
            {
                Console.WriteLine(varobj0.ToString());
            }

            dt1.CloneArray(varjson0);

            Console.WriteLine(dt1.Rows.Count);

            using var data = dbHelper.ExecuteDataSetAsync("SELECT TOP (1000) * FROM [HKR51].[dbo].[XGDZ_CheckSub];SELECT * FROM [HKR51].[dbo].[XGDZ_cUser];SELECT * FROM [HKR51].[dbo].[XGDZ_cUserQx]").Result;

            var array0 = data.Tables[0].ToArray();
            array0.ToJson();

            using (var reader = dbHelper.ExecuteReader(CommandType.Text, "SELECT TOP (1000) * FROM [HKR51].[dbo].[XGDZ_CheckSub];SELECT * FROM [HKR51].[dbo].[XGDZ_cUser];SELECT * FROM [HKR51].[dbo].[XGDZ_cUserQx]")) 
            {
                var dataTable = reader.GetDataTableAsync().Result;
                var dataset = reader.GetDataSetAsync().Result;
            }

            var dataset0 = dbHelper.GetMessageForDataSetAsync("usp_output", new { bookname = "T001" }, dbHelper.GetOutParam("recordCount", typeof(int))).Result;

            var dbParameters = dbHelper.GetSpParameterSetAsync("usp_output").Result;

            var dbParameters0 = dbHelper.GetSpParameterSetAsync("usp_output1").Result;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(AllException);
            }

            //int a = 0, b = 0, c = 0, d = 0;

            //KeepAlive keep = new(1, async () =>
            //{
            //    Console.Clear();
            //    await Console.Out.WriteLineAsync(string.Format("情况：{0}，{1}，{2}, 成功 {3},超时 {4},其他 {5},http计数：{6}", ThreadPool.ThreadCount, ThreadPool.PendingWorkItemCount, ThreadPool.CompletedWorkItemCount, a, b, c, d));
            //});

            app.UseDiySession();

            app.UseIgnoreUrl("Views/Cs", "Views", "cs.html");

            app.UseStaticFiles(); //"/Raa"

            app.UseRouting();

            app.UseEndpoints(e =>
            {
                //e.MapGet("/", async (context) =>
                //{
                //    context.Session.SetAvailable(!context.Session.IsAvailable);

                //    Interlocked.Increment(ref d);
                //    ApiPacket packet = new(1, 250, 10000);
                //    packet.Set("a", StringExtension.GetGuid());
                //    var s = await client.SendAsync(packet);

                //    string msg;
                //    switch (s.OnNetFrame)
                //    {
                //        case NetFrameState.Success:
                //            Interlocked.Increment(ref a);
                //            msg = "OK";
                //            break;
                //        case NetFrameState.Timeout:
                //            Interlocked.Increment(ref b);
                //            msg = "NO";
                //            break;
                //        default:
                //            Interlocked.Increment(ref c);
                //            msg = "SB";
                //            break;
                //    }

                //    await context.Response.WriteAsync(msg);
                //});

                e.MapAshxs(routes =>
                {
                    routes.MapApiRoute(areaName: "WebTestApp.Api",
                        template: "Api/{controller=GetMin}/{action=GetSql}/{id1?}");

                    routes.MapApiRoute(areaName: "WebTestApp.ApiView",
                        //controller: "Heheh",
                        //action: "Index",
                        template: "{controller=Heheh}/{action=Index}/{id?}");

                    routes.MapApiRoute(areaName: "WebTestApp.Api",
                      controller: "GetMin",
                      template: "Api0/{action=GetSql}/{id1?}");

                    //routes.MapApiRoute(areaName: "WebTestApp.Api",
                    //    controller: null,
                    //    action: "GetSql",
                    //    template: "Api0/{controller=GetMin}/{id1?}");

                    routes.MapApiRoute(areaName: "WebTestApp.ApiView",
                        controller: null,
                        action: "As",
                        template: "cs/{controller}/{id?}");

                    routes.MapApiRoute(
                        areaName: "WebTestApp.ApiView",
                        controller: "Class",
                        action: "As",
                        template: "v2/{id?}");

                    //routes.MapApiRoute(template: "{controller=Class}/{action=As}/{id?}");
                });
            });

            //SqlConnection sqlConnection = new SqlConnection();

            //MongoDB.Driver.Core.Configuration

            //MongoDB.Driver.MongoClient client = new MongoDB.Driver.MongoClient("mongodb://127.0.0.1:27017");
            //MongoDB.Driver.IMongoDatabase db = client.GetDatabase("MongoDBDemo");
            //MySql.Data.MySqlClient.MySqlCommand
            //MySql.Data.MySqlClient.MySqlCommandBuilder()
            //MySql.Data.MySqlClient.MySqlParameter

            //app.UseSession();

            //Microsoft.AspNetCore.Session.SessionMiddleware;
            //Microsoft.AspNetCore.Session.DistributedSessionStore;

            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider("D:/Nixue工作室/WebUpload/wwwroot"),
            //    RequestPath = "/Raa"
            //});

            //app.UseAshx(routes =>
            //{
            //    routes.MapApiRoute(
            //        name: "default0",
            //        areaName: "WebTestApp.Api",
            //        template: "Api/{controller=GetMin}/{action=GetSql}/{id1?}");

            //    routes.MapApiRoute(
            //        name: "default1",
            //        areaName: "WebTestApp.Api",
            //        controller: "GetMin",
            //        template: "Api/{action=GetSql}/{id1?}");

            //    routes.MapApiRoute(
            //        name: "default2",
            //        areaName: "WebTestApp.ApiView",
            //        controller: "Heheh",
            //        action: "Index",
            //        template: "v/{id?}");

            //    routes.MapApiRoute(
            //        name: "default3",
            //        areaName: null,
            //        controller: "Heheh",
            //        action: "Index",
            //        template: "v1/{id?}");

            //    routes.MapApiRoute(
            //        name: "default4",
            //        areaName: "WebTestApp.ApiView",
            //        controller: "Class",
            //        action: "As",
            //        template: "v2/{id?}");
            //});

            //Microsoft.AspNetCore.Mvc.Routing.MvcRouteHandler//MvcBuilder//AttributeRouting.CreateAttributeMegaRoute(app.ApplicationServices)

            //app.UseMvc(routes =>
            // {
            //     routes.MapRoute(
            //         name: "default",
            //         template: "{controller=Home}/{action=Index}/{id?}");
            // });

            //app.UseSession();

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }

        public async Task AllException(HttpContext context, Exception exception)
        {
            await context.Response.WriteAsync("An unknown error has occurred!");
            Log.Error("捕获全局异常：", exception);
        }
    }
}
