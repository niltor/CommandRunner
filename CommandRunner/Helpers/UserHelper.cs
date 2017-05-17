using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CommandRunner.Helpers
{
	public class UserHelper
	{
		public static async Task<UserInfo> GetUserAsync()
		{
			if (File.Exists("./user.lock"))
			{
				var file = new FileInfo("./user.lock");
				StreamReader stream = file.OpenText();

				string content = stream.ReadToEnd();
				// UserInfo is null,Init it;
				if (String.IsNullOrEmpty(content))
				{
					stream.Dispose();
					await InitUser();
				}
				else
				{
					try
					{
						UserInfo user = JsonConvert.DeserializeObject<UserInfo>(content);
						if (String.IsNullOrEmpty(user.UserName) || String.IsNullOrEmpty(user.Password))
						{
							//UserInfo is null,reinit it.
							await InitUser();
						}
						else
						{
							return user;
						}
					}
					catch (Exception)
					{
						// not valid UserInfo,reinit it.
						await InitUser();
					}
					finally
					{
						stream.Dispose();

					}
				}
			}
			else
			{
				await InitUser();
			}
			return default(UserInfo);
		}

		// Initial User
		public static async Task InitUser()
		{
			var file = new FileInfo("./user.lock");
			FileStream stream = file.OpenWrite();
			var defaultUser = new UserInfo
			{
				UserName = "admin",
				Password = BitConverter.ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes("MSDev.cc")))
			};

			Byte[] jsonBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(defaultUser));
			await stream.WriteAsync(jsonBytes, 0, jsonBytes.Length);

			stream.Dispose();
		}

		public static async Task EditUserAsync(UserInfo user)
		{
			var file = new FileInfo("./user.lock");
			FileStream stream = file.OpenWrite();
			byte[] jsonBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(user));

			Console.WriteLine(jsonBytes.ToString());
			await stream.WriteAsync(jsonBytes, 0, jsonBytes.Length);

			stream.Dispose();
		}

	}

	public class UserInfo
	{
		public string UserName { get; set; }
		public string Password { get; set; }

	}


}
