using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Cryptography;

namespace ServerFileApp
{
    class Program
    {
        const string constring = @"Data Source=BOBBIE\SQLExpress;Initial Catalog=EasyEncryption;Integrated Security=True";

        static void Main(string[] args)
        {
            TcpListener server = new TcpListener(IPAddress.IPv6Any, 8080);
            TcpClient client = default(TcpClient);

            server.Start();
            Console.WriteLine("Server has started...");



            while (true)
            {
                client = server.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream);

                // Read 'intent' from client

                string intent = reader.ReadLine();

                switch (intent)
                {
                    case "Upload":
                        string filename = decryptInfo(reader.ReadLine());
                        int size = Int32.Parse(reader.ReadLine());
                        string owner = decryptInfo(reader.ReadLine());
                        string share = decryptInfo(reader.ReadLine());
                        string originalfilename2 = reader.ReadLine();
                        Console.WriteLine("Encrypted filename: " + originalfilename2);
                        string originalfilename = decryptInfo(originalfilename2);
                        Console.WriteLine("Decrypted filename: " + originalfilename);
                        string originalfileext = decryptInfo(reader.ReadLine());
                        string encryptedkey = reader.ReadLine();
                        string IV = reader.ReadLine();

                        uploadFiles(filename, size, share, owner, originalfilename, originalfileext,encryptedkey,IV);
                        
                        //stream.CopyTo(new FileStream(@"D:\filetransfer\" + filename, FileMode.Create, FileAccess.Write));
                        break;

                    case "Retrieve":
                        string retrievinguser = reader.ReadLine();
                        DataTable dt = retrieveFiles(retrievinguser);
                        string xml = SerializeTableToString(dt);
                        string userpubkey = getUserPubKey(retrievinguser);
                        Console.WriteLine("pub key : " + userpubkey + "\n\n\n");
                        using (StreamWriter sw = new StreamWriter(stream))
                        {
                            byte[] xmlarray = Encoding.UTF8.GetBytes(xml);
                            byte[] key = new byte[32];
                            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                            {
                                rng.GetBytes(key);
                                using (RijndaelManaged aes = new RijndaelManaged())
                                {
                                    aes.KeySize = 256;
                                    aes.Mode = CipherMode.CBC;
                                    aes.GenerateIV();
                                    aes.Key = key;
                                    using (var mems = new MemoryStream())
                                    {
                                        using (ICryptoTransform encryptor = aes.CreateEncryptor())
                                        {
                                            using (var cryptostream = new CryptoStream(mems,encryptor,CryptoStreamMode.Write))
                                            {
                                                cryptostream.Write(xmlarray, 0, xmlarray.Length);
                                                cryptostream.FlushFinalBlock();
                                                using (RSACryptoServiceProvider rsaxml = new RSACryptoServiceProvider())
                                                {
                                                    byte[] encryptedxml = mems.ToArray();
                                                    rsaxml.FromXmlString(userpubkey);
                                                    byte[] encryptedxmlkey = rsaxml.Encrypt(key, false);
                                                    string base64xml = Convert.ToBase64String(encryptedxml);
                                                    string base64key = Convert.ToBase64String(encryptedxmlkey);
                                                    string base64IV = Convert.ToBase64String(aes.IV);
                                                    sw.WriteLine(base64key);
                                                    sw.WriteLine(base64IV);
                                                    sw.WriteLine(base64xml);
                                                    sw.Flush();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    case "Download":
                        //Download files

                        Console.WriteLine("Download request received");

                        string user = reader.ReadLine();
                        int filecount = Int32.Parse(reader.ReadLine());
                        Console.WriteLine("Downloading user: " + user);
                        string files = "";
                        string owners = "";
                        string sharedGroups = "";
                        string pubkey = getUserPubKey(user);
                        StreamWriter swd = new StreamWriter(stream);
                        FileItem fi = new FileItem();
                        using (RSACryptoServiceProvider rsap = new RSACryptoServiceProvider()) {
                            rsap.FromXmlString(pubkey);
                            for (int i = 0; i < filecount; i++)
                            {
                                files = (reader.ReadLine());
                                owners = (reader.ReadLine());
                                sharedGroups = (reader.ReadLine());
                                fi = downloadFile(owners, files, sharedGroups);
                                fi.EncKey = (decryptKey(owners, files, pubkey));

                                swd.WriteLine(Convert.ToBase64String(rsap.Encrypt(Encoding.UTF8.GetBytes(fi.hashedFilename),false)));
                                swd.WriteLine(fi.IV);
                                swd.WriteLine(Convert.ToBase64String(rsap.Encrypt(Encoding.UTF8.GetBytes(fi.Originalfilename),false)));
                                swd.WriteLine(Convert.ToBase64String(rsap.Encrypt(Encoding.UTF8.GetBytes(fi.OriginalfileExt),false)));
                                swd.WriteLine(fi.EncKey);

                                swd.Flush();
                                addLogs(files, owners, user,sharedGroups);
                                Console.WriteLine("Logs for {0} downloading {1} owned by {2} added successfully!", user, files, owners);
                            }
                        }

                        break;

                    case "Pubkey":
                        Console.WriteLine("Server Pub Key request received");
                        CspParameters csp = new CspParameters();
                        csp.KeyContainerName = "EEKeys";
                        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(csp);
                        string pubkeyxml = rsa.ToXmlString(true);
                        Console.WriteLine("Server Pub Key: " + pubkeyxml);
                        using (StreamWriter streamwrite = new StreamWriter(stream))
                        {
                            streamwrite.WriteLine(pubkeyxml);
                        }
                        break;

                    case "Logs":
                        //Retrieve name,owner,shared and use serializate dt to send back logs.
                        string itemname = reader.ReadLine();
                        string itemgroup = reader.ReadLine();
                        string itemowner = reader.ReadLine();
                        string loguser = reader.ReadLine();
                        string loguserpubkey = getUserPubKey(loguser);
                        string logxml = SerializeTableToString(retrieveLogs(itemname, itemowner, itemgroup));
                        Console.WriteLine(logxml);
                        using (StreamWriter logsw = new StreamWriter(stream))
                        {
                            byte[] xmlarray = Encoding.UTF8.GetBytes(logxml);
                            byte[] key = new byte[32];
                            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                            {
                                rng.GetBytes(key);
                                using (RijndaelManaged aes = new RijndaelManaged())
                                {
                                    aes.KeySize = 256;
                                    aes.Mode = CipherMode.CBC;
                                    aes.GenerateIV();
                                    aes.Key = key;
                                    using (var mems = new MemoryStream())
                                    {
                                        using (ICryptoTransform encryptor = aes.CreateEncryptor())
                                        {
                                            using (var cryptostream = new CryptoStream(mems, encryptor, CryptoStreamMode.Write))
                                            {
                                                cryptostream.Write(xmlarray, 0, xmlarray.Length);
                                                cryptostream.FlushFinalBlock();
                                                using (RSACryptoServiceProvider rsaxml = new RSACryptoServiceProvider())
                                                {
                                                    byte[] encryptedxml = mems.ToArray();
                                                    rsaxml.FromXmlString(loguserpubkey);
                                                    byte[] encryptedxmlkey = rsaxml.Encrypt(key, false);
                                                    string base64xml = Convert.ToBase64String(encryptedxml);
                                                    string base64key = Convert.ToBase64String(encryptedxmlkey);
                                                    string base64IV = Convert.ToBase64String(aes.IV);
                                                    logsw.WriteLine(base64key);
                                                    logsw.WriteLine(base64IV);
                                                    logsw.WriteLine(base64xml);
                                                    logsw.Flush();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        break;
                }
                client.Close();
                stream.Close();
            }
        }

        public static DataTable retrieveLogs(string name, string owner, string group)
        {
            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM AccessLogs WHERE OriginalFilename = @name AND Owner = @owner AND sharedGroup = @group"))
                {
                    cmd.Parameters.AddWithValue("@owner", owner);
                    cmd.Parameters.AddWithValue("@group", group);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        sda.SelectCommand = cmd;
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        return dt;
                    }
                }
            }
        }


        public static string decryptInfo(string info)
        {
            CspParameters csp = new CspParameters();
            csp.KeyContainerName = "EEKeys";
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(csp))
            {
                return Encoding.UTF8.GetString(rsa.Decrypt(Convert.FromBase64String(info), false));
            }
        }

        public static string decryptKey(string owner, string filename, string pubkey)
        {
            CspParameters csp = new CspParameters();
            csp.KeyContainerName = "EEKeys";
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(csp))
            {
                string enckey = "";
                using (SqlConnection con = new SqlConnection(constring))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT EncKey FROM [Files] WHERE Owner = @Owner AND OriginalFilename + OriginalFileExt = @filename"))
                    {
                        cmd.Parameters.AddWithValue("@Owner", owner);
                        cmd.Parameters.AddWithValue("@filename", filename);
                        cmd.Connection = con;
                        cmd.Connection.Open();
                        using (SqlDataReader rd = cmd.ExecuteReader())
                        {
                            if (rd.HasRows)
                            {
                                rd.Read();
                                enckey = rd.GetString(0);
                            }
                        }
                    }
                }
                byte[] deckey = rsa.Decrypt(Convert.FromBase64String(enckey), false);
                rsa.FromXmlString(pubkey);
                byte[] reenckey = rsa.Encrypt(deckey, false);
                return Convert.ToBase64String(reenckey);
            }
        }

        public static string getUserPubKey(string user)
        {
            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT PubKey FROM [User] WHERE Username = @user"))
                {
                    cmd.Parameters.AddWithValue("@user", user);
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            rd.Read();
                        }
                        return rd.GetString(0);
                    }
                }
            }
        }

        public static FileItem downloadFile(string owner,string filename,string share)
        {
            FileItem fi = new FileItem();
            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT HashedFilename,IV,OriginalFilename,OriginalFileExt FROM [Files] WHERE Owner = @owner AND OriginalFilename + OriginalFileExt = @filename AND SharedGroups = @share"))
                {
                    cmd.Parameters.AddWithValue("@owner", owner);
                    cmd.Parameters.AddWithValue("@filename", filename);
                    cmd.Parameters.AddWithValue("@share", share);
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                fi.hashedFilename = rd.GetString(0);
                                fi.IV = rd.GetString(1);
                                fi.Originalfilename = rd.GetString(2);
                                fi.OriginalfileExt = rd.GetString(3);
                            }
                        }
                        return fi;
                    }
                }
            }
        }

        public static void addLogs(string filename, string owner, string downloader,string group)
        {
            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO AccessLogs (OriginalFilename,Owner,UserDownload,sharedGroup) VALUES (@filename,@owner,@user,@group)"))
                {
                    cmd.Parameters.AddWithValue("@filename", filename);
                    cmd.Parameters.AddWithValue("@owner", owner);
                    cmd.Parameters.AddWithValue("@user", downloader);
                    cmd.Parameters.AddWithValue("@group", group);
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void uploadFiles(string filename, int size, string group, string owner, string originalfilename, string originalfileext, string encryptedkey, string IV)
        {
            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Files (HashedFilename,Size,SharedGroups,Owner,OriginalFilename,OriginalFileExt,EncKey,IV) VALUES (@filename,@size,@group,@owner,@originalfilename,@originalfileext,@key,@IV)")) { 
                cmd.Parameters.AddWithValue("@filename", filename);
                cmd.Parameters.AddWithValue("@size", size);
                cmd.Parameters.AddWithValue("@group", group);
                cmd.Parameters.AddWithValue("@owner", owner);
                cmd.Parameters.AddWithValue("@originalfilename", originalfilename);
                cmd.Parameters.AddWithValue("@originalfileext", originalfileext);
                cmd.Parameters.AddWithValue("@key", encryptedkey);
                cmd.Parameters.AddWithValue("@IV", IV);
                cmd.Connection = con;
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
        }

        public static DataTable retrieveFiles(string username)
        {
            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT OriginalFilename + OriginalFileExt AS [Filename],Size,SharedGroups,Owner FROM Files WHERE Owner = @owner"))
                {
                    cmd.Parameters.AddWithValue("@owner", username);
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        sda.SelectCommand = cmd;
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public static string SerializeTableToString(DataTable dt)
        {
            using (var sw = new StringWriter())
            using (var tw = new XmlTextWriter(sw))
            {
                dt.TableName = @"AccessLogs";

                tw.Formatting = Formatting.Indented;

                tw.WriteStartDocument();
                tw.WriteStartElement(@"data");

                ((IXmlSerializable)dt).WriteXml(tw);

                tw.WriteEndElement();
                tw.WriteEndDocument();

                tw.Flush();
                tw.Close();
                sw.Flush();

                return sw.ToString();
            }
        }
    }
}
