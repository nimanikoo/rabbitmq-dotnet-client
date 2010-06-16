// This source code is dual-licensed under the Apache License, version
// 2.0, and the Mozilla Public License, version 1.1.
//
// The APL v2.0:
//
//---------------------------------------------------------------------------
//   Copyright (C) 2007-2010 LShift Ltd., Cohesive Financial
//   Technologies LLC., and Rabbit Technologies Ltd.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//---------------------------------------------------------------------------
//
// The MPL v1.1:
//
//---------------------------------------------------------------------------
//   The contents of this file are subject to the Mozilla Public License
//   Version 1.1 (the "License"); you may not use this file except in
//   compliance with the License. You may obtain a copy of the License at
//   http://www.rabbitmq.com/mpl.html
//
//   Software distributed under the License is distributed on an "AS IS"
//   basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
//   License for the specific language governing rights and limitations
//   under the License.
//
//   The Original Code is The RabbitMQ .NET Client.
//
//   The Initial Developers of the Original Code are LShift Ltd,
//   Cohesive Financial Technologies LLC, and Rabbit Technologies Ltd.
//
//   Portions created before 22-Nov-2008 00:00:00 GMT by LShift Ltd,
//   Cohesive Financial Technologies LLC, or Rabbit Technologies Ltd
//   are Copyright (C) 2007-2008 LShift Ltd, Cohesive Financial
//   Technologies LLC, and Rabbit Technologies Ltd.
//
//   Portions created by LShift Ltd are Copyright (C) 2007-2010 LShift
//   Ltd. Portions created by Cohesive Financial Technologies LLC are
//   Copyright (C) 2007-2010 Cohesive Financial Technologies
//   LLC. Portions created by Rabbit Technologies Ltd are Copyright
//   (C) 2007-2010 Rabbit Technologies Ltd.
//
//   All Rights Reserved.
//
//   Contributor(s): ______________________________________.
//
//---------------------------------------------------------------------------
using System;
using System.IO;
using System.Text;

using RabbitMQ.Client;
using RabbitMQ.Client.Content;
using RabbitMQ.Client.MessagePatterns;

namespace RabbitMQ.Client.Examples {
    public class AddClient {
        public static int Main(string[] args) {
            if (args.Length < 1) {
                Console.Error.WriteLine("Usage: AddClient <hostname>[:<portnumber>] [<number> ...]");
                Console.Error.WriteLine("RabbitMQ .NET client version "+typeof(IModel).Assembly.GetName().Version.ToString());
                return 2;
            }

            ConnectionFactory cf = new ConnectionFactory();
            cf.Address = args[0];           
 
            using (IConnection conn = cf.CreateConnection()) {
                using (IModel ch = conn.CreateModel()) {
                    
                    object[] addends = new object[args.Length - 1];
                    for (int i = 0; i < args.Length - 1; i++) {
                        addends[i] = double.Parse(args[i + 1]);
                    }

                    SimpleRpcClient client = new SimpleRpcClient(ch, "AddServer");
                    client.TimeoutMilliseconds = 5000;
                    client.TimedOut += new EventHandler(TimedOutHandler);
                    client.Disconnected += new EventHandler(DisconnectedHandler);
                    object[] reply = client.Call(addends);
                    if (reply == null) {
                        Console.WriteLine("Timeout or disconnection.");
                    } else {
                        Console.WriteLine("Reply: {0}", reply[0]);
                    }
                }
            }
            return 0;
        }

	public static void TimedOutHandler(object sender, EventArgs e) {
	    Console.WriteLine("Timed out.");
	}

	public static void DisconnectedHandler(object sender, EventArgs e) {
	    Console.WriteLine("Disconnected.");
	}
    }
}