using System;
using System.Collections.Generic;
using System.Text;

namespace CB_Bomberman.Client
{

    class ConsoleInterface
    {
        private int logSize;
        public Dictionary<string, string> blocks = new Dictionary<string, string>();
        public LinkedList<string> log = new LinkedList<string>();

        public ConsoleInterface(int logSize = 10)
        {
            this.logSize = logSize;
            //Console.SetWindowSize(50, 20);
        }

        public void NewLogLine(string line)
        {
            log.AddLast(line);
            if(log.Count > logSize) log.RemoveFirst();
            Print();
        }

        public void Block(string id, string block)
        {
            blocks[id] = block;
        }

        public void Print()
        {
            Console.SetCursorPosition(0,0);
            Console.WriteLine("Latest moves:");

            LinkedListNode<string> current = log.First;
            while (current != null)
            {
                Console.WriteLine(current.Value + "                 ");
                current = current.Next;
            }
        }

    }
}
