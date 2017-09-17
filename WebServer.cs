using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using MongoDB.Driver;
using MongoDB.Bson;

namespace ServerConsole
{
    public delegate void RouteHandle(HttpListenerContext context, string _rootDirectory);

    public class WebServer
    {
        public WebServer()
        {
            this.Initialize();
        }

        private string _rootDirectory;
        private int _port;
        private Thread _serverThread;
        private HttpListener _httpListener;

        public void Close()
        {
            _httpListener.Close();
        }

        private void Initialize()
        {
            this._rootDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            this._port = 8888;
            this._serverThread = new Thread(this.Listen);
            this._serverThread.Start();
        }

        private void Listen()
        {
            this._httpListener = new HttpListener();
            this._httpListener.Prefixes.Add("http://*:" + _port.ToString() + "/");
            this._httpListener.Start();
            Console.WriteLine("Starting to listen for http requests.");
            while(true)
            {
                try
                {
                    HttpListenerContext context = this._httpListener.GetContext();
                    Process(context);
                }
                catch (Exception)
                {

                }
            }
        }

        private void Process(HttpListenerContext context)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("Method: " + context.Request.HttpMethod + " - " + context.Request.Url.AbsolutePath);
            /*
            Console.WriteLine("ContentType: " + context.Request.ContentType);
            Console.WriteLine("Cookies: " + context.Request.Cookies);
            Console.WriteLine("Headers:\n" + context.Request.Headers);
            Console.WriteLine("AbsolutePath: " + context.Request.Url.AbsolutePath);
            Console.WriteLine("HasEntityBody: " + context.Request.HasEntityBody);
            Console.WriteLine("LocalEndPoint: " + context.Request.LocalEndPoint);
            Console.WriteLine("RemoteEndPoint: " + context.Request.RemoteEndPoint);
            */

            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string filepathRequested = request.Url.AbsolutePath;

            response.AddHeader("Access-Control-Allow-Origin", "*");

            // -- Ignore favicon
            if(filepathRequested.Contains("favicon.ico"))
                goto END;

            // -- Specific routes
            bool usingSpecificRoute = false;
            try
            {
                routeHandler[filepathRequested].handle(context, _rootDirectory);
                usingSpecificRoute = true;
            }
            catch (Exception)
            {
                Console.WriteLine("Cant find Specific route.");
            }

            if(usingSpecificRoute)
                goto END;

            // -- Open file and send contents
            SendFile(context, filepathRequested, _rootDirectory);

            END:

            response.OutputStream.Flush();
            response.OutputStream.Close();
        }

        private static void SendFile(HttpListenerContext context, string filepathRequested, string _rootDirectory)
        {
            Console.WriteLine("Sending file at " + filepathRequested);

            if(filepathRequested.StartsWith('/'))
                filepathRequested = filepathRequested.Substring(1);

            string filename = Path.Combine(_rootDirectory, "site/" + filepathRequested);
            try
            {
                FileStream fileStream = new FileStream(filename, FileMode.Open);

                byte[] buffer = new byte[1024 * 16]; // -- 16 KB
                int nbytes;
                while ((nbytes = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    context.Response.OutputStream.Write(buffer, 0, nbytes);
                }
                fileStream.Close();
            }
            catch (Exception)
            {
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            }

            context.Response.OutputStream.Flush();
            context.Response.OutputStream.Close();
        }

        private static void SendData(HttpListenerContext context, string data)
        {
            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(data);

                context.Response.OutputStream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception)
            {
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            }

            context.Response.OutputStream.Flush();
            context.Response.OutputStream.Close();
        }

        private static string GetPostBody(HttpListenerRequest request)
        {
            using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                return reader.ReadToEnd();
            }
        }

        private static Dictionary<string, string> mimeTypeMapping = new Dictionary<string, string>()
        {
            { "txt", "text/plain" },
            { "json", "application/json" }
        };

        private static Dictionary<string, Route> routeHandler = new Dictionary<string, Route>()
        {
            { "/api/status", new Route(delegate(HttpListenerContext context, string _rootDirectory) {
                Console.WriteLine("Sending status.");
                context.Response.ContentType = "application/json";
                SendData(context, "{ \"status\": \"online\" }");
            }) },
            { "/api/units", new Route(delegate(HttpListenerContext context, string _rootDirectory) {
                Console.WriteLine("Should send units");
            }) },
            { "/api/itemblueprints", new Route(delegate(HttpListenerContext context, string _rootDirectory) {

                switch (context.Request.HttpMethod)
                {
                    case "GET":
                        Console.WriteLine("GET request");
                        //string doc = DatabaseManager.GetCollection("ItemBlueprints").FindSync(Builders<BsonDocument>.Filter.Eq("_id", new ObjectId("59abc5141be3e86572ec2438"))).First().ToJson();
                        //SendData(context, doc);
                        break;
                    case "POST":
                        Console.WriteLine("POST request " + GetPostBody(context.Request));
                        //string doc2 = DatabaseManager.GetCollection("ItemBlueprints").FindSync(Builders<BsonDocument>.Filter.Eq("_id", new ObjectId("59abc5141be3e86572ec2438"))).First().ToJson();
                        //SendData(context, doc2);
                        break;
                }
            }) }
        };

    }

    public class Route
    {
        public RouteHandle handle;

        public Route(RouteHandle handle)
        {
            this.handle = handle;
        }
    }
}
