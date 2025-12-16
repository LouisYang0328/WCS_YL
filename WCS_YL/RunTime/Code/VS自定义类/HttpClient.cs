using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using log4net;
using System.Security.Cryptography;


public class HttpClient
{
	public HttpClient()
	{
	}

	public string Call(string param, string url, out string errorMsg)
	{
		errorMsg = string.Empty;
		try
		{
			byte[] byts = Encoding.UTF8.GetBytes(param);

			HttpWebRequest request = WebRequest.CreateHttp(url) as HttpWebRequest;
			request.Method = "POST";
			request.ContentType = "application/json";
			request.ContentLength = byts.Length;

			using (Stream s = request.GetRequestStream())
			{
				s.Write(byts, 0, byts.Length);
			}
			using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
			{
				using (Stream s = response.GetResponseStream())
				{
					using (StreamReader sr = new StreamReader(s))
					{
						string json = sr.ReadToEnd();
						return json;
					}
				}
			}
		}
		catch (WebException ex)
		{
			errorMsg = ex.Message;
			string text = string.Empty;
			if (ex.Status == WebExceptionStatus.ProtocolError)
			{
				using (Stream data = ex.Response.GetResponseStream())
					using (var reader = new StreamReader(data))
				{
					text = reader.ReadToEnd();
				}
			}
			errorMsg = ex.Message;
			return text;
		}
	}

	public string CalculateMD5Hash(string input)
	{
		using (MD5 md5 = MD5.Create())
		{
			byte[] inputBytes = Encoding.UTF8.GetBytes(input);
			byte[] hashBytes = md5.ComputeHash(inputBytes);

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < hashBytes.Length; i++)
			{
				sb.Append(hashBytes[i].ToString("x2"));
			}

			return sb.ToString();
		}
	}

	public async Task<string> CallAsync(string type, string param, string url)
	{
		HttpWebRequest request = WebRequest.CreateHttp(url) as HttpWebRequest;
		try
		{
			request.Method = type;
			request.ContentType = "application/json";
			request.Timeout = 150000;
			request.AllowAutoRedirect = false;
			if (type == "post" || type == "put")
			{
				byte[] byts = Encoding.UTF8.GetBytes(param);
				request.ContentLength = byts.Length;
				using (Stream s = request.GetRequestStream())
				{
					s.Write(byts, 0, byts.Length);
				}
			}

			using (var response = request.GetResponseAsync().Result as HttpWebResponse)
			{
				using (var s = response.GetResponseStream())
				{
					using (var sr = new StreamReader(s))
					{
						return await sr.ReadToEndAsync().ConfigureAwait(false);
					}
				}
			}

		}
		catch (Exception ex)
		{
			// logger.Error($"HTTP数据异常.{ex.Message}");
			return null;
		}
		finally
		{
			request.Abort();
		}
	}


	///// <summary>
	///// 通用请求方法
	///// </summary>
	///// <param name="url"></param>
	///// <param name="data"></param>
	///// <param name="method">POST GET PUT DELETE</param>
	///// <param name="contentType">"POST application/x-www-form-urlencoded; charset=UTF-8"</param>
	///// <param name="encoding"></param>
	///// <returns></returns>
	//public static string HttpRequest(string url, string data, string method = "PUT", string contentType = "application/json", Encoding encoding = null)
	//{
	//    byte[] datas = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(data);//data可以直接传字节类型 byte[] data,然后这一段就可以去掉
	//    if (encoding == null)
	//    {
	//        encoding = Encoding.UTF8;
	//    }
	//    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
	//    request.Method = method;
	//    request.Timeout = 150000;
	//    request.AllowAutoRedirect = false;
	//    if (!string.IsNullOrEmpty(contentType))
	//    {
	//        request.ContentType = contentType;
	//    }
	//    if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
	//    {
	//        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
	//    }
	//    Stream requestStream = null;
	//    string responseStr = null;
	//    try
	//    {
	//        if (datas != null)
	//        {
	//            request.ContentLength = datas.Length;
	//            requestStream = request.GetRequestStream();
	//            requestStream.Write(datas, 0, datas.Length);
	//            requestStream.Close();
	//        }
	//        else
	//        {
	//            request.ContentLength = 0;
	//        }
	//        using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
	//        {
	//            Stream getStream = webResponse.GetResponseStream();
	//            byte[] outBytes = ReadFully(getStream);
	//            getStream.Close();
	//            responseStr = Encoding.UTF8.GetString(outBytes);
	//        }
	//    }
	//    catch (Exception)
	//    {
	//        throw;
	//    }
	//    finally
	//    {
	//        request = null;
	//        requestStream = null;
	//    }
	//    return responseStr;
	//}

	//private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
	//{
	//    return true; //总是接受
	//}

}

