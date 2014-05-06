using ProtoBuf;
using Server.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Server.ProtoGeneration
{
    class LdpProtoGenerator
    {
        private static string JAVA_PROTOCOL_NAME = "LdpProtocol";
        private static string PROTO_MESSAGE_NAME = "messages.proto";
        private static string GetProtoFolder()
        {
            try
            {
                string path = "";
                string binDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                int subfolders = 5;
                int iter = 0;
                while (iter != subfolders)
                {
                    path = System.IO.Path.GetDirectoryName(binDirectory);
                    binDirectory = path;
                    iter++;
                }
                return path + "\\Protocol";
            }
            catch (Exception ex)
            {
                string exc = String.Format("GetProtoFolder exception: {0}.", ex.Message);
                MessageBox.Show(exc, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return String.Empty;
            }

        }

        private static string GetProtoString()
        {
            try
            {
                string proto = Serializer.GetProto<LdpPacket>();
                string[] splittet = proto.Split('\n');
                splittet[0] = "option java_outer_classname = \"" + JAVA_PROTOCOL_NAME + "\";" + "\r\n";
                splittet[0] += "option optimize_for = SPEED;" + "\r\n";

                return String.Join("", splittet);
            }
            catch (Exception ex)
            {
                string exc = String.Format("GetProtoString exception: {0}.", ex.Message);
                MessageBox.Show(exc, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return String.Empty;
            }

        }

        public static void GenerateProtoJava()
        {
            string path = GetProtoFolder() + "\\" + PROTO_MESSAGE_NAME;
            string proto = GetProtoString();
            string mess = "";
            try
            {
                if (path != String.Empty && proto != String.Empty)
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    FileStream fs = File.Create(path);
                    fs.Close();
                    TextWriter tw = new StreamWriter(path);
                    tw.Write(proto);
                    tw.Close();

                    mess = String.Format("Proto generation successfully complete.\nPath={0}.\nProtoName={1}.",
                        path, PROTO_MESSAGE_NAME);
                    MessageBox.Show(mess, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (path == String.Empty)
                        mess = "ProtoFolder path is empty.";
                    else
                        mess = "ProtoString is empty.";
                    MessageBox.Show(mess, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                mess = String.Format("GenerateProtoJava exception: {0}.", ex.Message);
                MessageBox.Show(mess, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
