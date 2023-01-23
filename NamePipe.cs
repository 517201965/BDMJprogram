using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;

namespace BDMJprogram
{
    class NamePipe
    {
        public void ConnectToServer()
        {
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "testpipe", PipeDirection.InOut))
            {
                pipeClient.Connect();

                using (StreamReader sr = new StreamReader(pipeClient))
                {
                    var data = new byte[10240];
                    data = System.Text.Encoding.Default.GetBytes("send to server");
                    pipeClient.Write(data, 0, data.Length);

                    string temp;
                    while ((temp = sr.ReadLine()) == null) ;
                    Console.WriteLine(temp);                    
                }
            }
        }
    }
}
