
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace JanTingQi
{
    class Program
    {

        static HttpListener httpobj;
        static string httpurlh5 = "";
        static void Main(string[] args)
        {
            
            //提供一个简单的、可通过编程方式控制的 HTTP 协议侦听器。此类不能被继承。
            httpobj = new HttpListener();
            //定义url及端口号，通常设置为配置文件
            //httpurlpc = ConfigurationManager.AppSettings["httpurlpc"];
            httpurlh5 = ConfigurationManager.AppSettings["httpurlh5"];
           // httpobj.Prefixes.Add(httpurlpc);
            httpobj.Prefixes.Add(httpurlh5);
            //启动监听器
            httpobj.Start();
            //异步监听客户端请求，当客户端的网络请求到来时会自动执行Result委托
            //该委托没有返回值，有一个IAsyncResult接口的参数，可通过该参数获取context对象
            httpobj.BeginGetContext(Result, null);
            Console.WriteLine($"服务端初始化完毕，正在等待客户端请求,时间：{DateTime.Now.ToString()}\r\n");
            Console.ReadKey();
            
        }
        private static void Result(IAsyncResult ar)
        {
            //当接收到请求后程序流会走到这里
            //继续异步监听
            httpobj.BeginGetContext(Result, null);
            var guid = Guid.NewGuid().ToString();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"接到新的请求:{guid},时间：{DateTime.Now.ToString()}");
            //获得context对象
            var context = httpobj.EndGetContext(ar);
            var request = context.Request;
            var response = context.Response;
            //context.Response.ContentType = "text/html;charset=UTF-8";//告诉客户端返回的ContentType类型为纯文本格式，编码为UTF-8
            //context.Response.AddHeader("Content-type", "text/html");//添加响应头信息



            context.Response.ContentType = "text/xml;charset=UTF-8";//告诉客户端返回的ContentType类型为纯文本格式，编码为UTF-8
            context.Response.AddHeader("Content-type", "text/xml");//添加响应头信息
            context.Response.ContentEncoding = Encoding.UTF8;
            //string returnObj = null;//定义返回客户端的信息
            //处理客户端发送的请求并返回处理信息
            string v = HandleRequest(request, response);
            if (v != null)
            {
                var returnByteArr = Encoding.UTF8.GetBytes(v);//设置客户端返回信息的编码
                response.Redirect(v);
            }
            response.Close();
        }

        private static string HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {            
                Dictionary<string, string> map = new Dictionary<string, string>();
                //接收客户端传过来的数据并转成字符串类型
                string rawUrl = request.RawUrl;
                string url1 = rawUrl.Substring(4);
                string[] strings = Regex.Split(url1, "&", RegexOptions.IgnoreCase);
                foreach (string i in strings)
                {
                    Console.WriteLine(i);
                }
            }
            catch (Exception ex)
            {
                response.StatusDescription = "404";
                response.StatusCode = 404;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("在接收数据时发生错误:{ex.ToString()}");
                //return "在接收数据时发生错误:{ex.ToString()}";//把服务端错误信息直接返回可能会导致信息不安全，此处仅供参考
            }
            return null;

        }
    }
}
