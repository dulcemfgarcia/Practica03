using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classlibrary
{
    public class Node<T>
    {
        public T value;
        public Node<T> LeftNode;
        public Node<T> RightNode;
        public int BF; //valor que controla el factor de equilibrio 

        public Node()
        {
            LeftNode = null;
            RightNode = null;
            BF = 0;
        }

    }
}
