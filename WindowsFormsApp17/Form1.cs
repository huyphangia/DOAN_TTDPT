using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Priority_Queue;

namespace WindowsFormsApp17
{
    public partial class Form1 : Form
    {
        enum AlgorithmType
        {
            HuffmanCoding, LZWcoding, ArithmeticCoding, None
        }
        public Form1()
        {
            InitializeComponent();
        }

        AlgorithmType algorithm = AlgorithmType.None;
        int thuattoan = 0;
        string filename = "";
        string filesavename = "";
        char black_box = '\u25a0';
        IDictionary<char, int> frequency_table = new Dictionary<char, int>();
        IDictionary<char, double> probility_table = new Dictionary<char, double>();

        struct mang
        {
            public char kitu;
            public int tanso;
        };
        struct node
        {
            public int left, right;
            public char kitu;
        };
        node[] tudien = new node[20000000];
        int goc;

        struct arimetic_node
        {
            public double low, high;
            public char kitu;
        }
        arimetic_node[] tudien_arimetic = new arimetic_node[2000000];

        string[] tudien_lzw = new string[200000];

        void Q_sort(mang[] kytu,int dau,int cuoi)
        {
            int i = dau, j = cuoi;
            int k = kytu[(dau + cuoi) / 2].tanso;
            do
            {
                while (kytu[i].tanso < k) ++i;
                while (kytu[j].tanso > k) --j;
                if (i <= j)
                {
                    mang tg;
                    tg = kytu[i];
                    kytu[i] = kytu[j];
                    kytu[j] = tg;
                    ++i;--j;
                }
            } while (i <= j);
            if (dau < j) Q_sort(kytu, dau, j);
            if (i < cuoi) Q_sort(kytu, i, cuoi);
        }

        string tim_kiem(int c, char a, string kq)
        {
            string kq1="", kq2 = "";
            if (tudien[c].kitu == '\0')
            {
                kq1 = tim_kiem(tudien[c].left, a, kq + '0');
                if (kq1 == "") kq2 = tim_kiem(tudien[c].right, a, kq + '1');
            }
            else if (tudien[c].kitu == a) return kq;
            if (kq1 != "") return kq1; else return kq2;
        }

        private class HuffmanNode
        {
            public char character;
            public int frequency;
            public HuffmanNode leftNode; 
            public HuffmanNode rightNode;
            public HuffmanNode(char cha = '\0', int freq = 0)
            {
                character = cha;
                frequency = freq;
                leftNode = null;
                rightNode = null;
            }
    
        };

        IDictionary<char, int> frequency_gathering(string data)
        {
            IDictionary<char, int> table = new Dictionary<char, int>();
            foreach(char c in data)
            {
                if (table.ContainsKey(c))
                {
                    table[c]++;
                }
                else
                {
                    table.Add(c, 1);
                }
            }
            return table;
        }

        HuffmanNode buildHuffmanTree(IDictionary<char, int> table)
        {
            
            SimplePriorityQueue<HuffmanNode> priorityQueue = new SimplePriorityQueue<HuffmanNode>();
            foreach(var kvp in table)
            {
                HuffmanNode huffmanNode = new HuffmanNode(kvp.Key, kvp.Value);
                priorityQueue.Enqueue(huffmanNode, huffmanNode.frequency);
            }
            while(priorityQueue.Count > 1)
            {
                HuffmanNode huffmanNode = new HuffmanNode();
                huffmanNode.leftNode = priorityQueue.Dequeue();
                huffmanNode.rightNode = priorityQueue.Dequeue();
                huffmanNode.frequency = huffmanNode.leftNode.frequency + huffmanNode.rightNode.frequency;
                priorityQueue.Enqueue(huffmanNode, huffmanNode.frequency);
            }
            return priorityQueue.Dequeue();
        }

        void travelHuffmanTree(HuffmanNode node, IDictionary<char, string> huffmanCode, string path)
        {
            if(node.leftNode == null && node.rightNode == null)
            {
                huffmanCode.Add(node.character, path);
                Console.WriteLine("{0}: {1}", node.character, path);
                return;
            }
            path += '0';
            travelHuffmanTree(node.leftNode, huffmanCode, path);
            path = path.Remove(path.Length - 1);

            path += '1';
            travelHuffmanTree(node.rightNode, huffmanCode, path);
            path = path.Remove(path.Length - 1);
        }

        void encodeHuffmanCode(string data)
        {
            string string_huffmanCode = "";
            //we gather frequency of all character
            IDictionary<char, int> table = frequency_gathering(data);
            frequency_table = table;

            //build huffman tree
            HuffmanNode root = buildHuffmanTree(table);

            //travel tree and print Huffmancode
            IDictionary<char, string> huffmanCode = new Dictionary<char, string>();
            string path = "";
            travelHuffmanTree(root, huffmanCode, path);
            foreach(KeyValuePair<char, string> kvp in huffmanCode) {
                string_huffmanCode += String.Format("{0}: {1}\r\n", kvp.Key, kvp.Value);
            }
            textBox3.Text = string_huffmanCode;

            //output encoded file
            string encodedData = "";
            foreach(char character in data)
            {
                encodedData += huffmanCode[character];
            }
            textBox2.Text = encodedData;
        }
        

        

        private void button2_Click(object sender, EventArgs e)
        {
            algorithm = AlgorithmType.HuffmanCoding;
            string lines = "";
            if (System.IO.File.Exists(filename))
            {
                string[] line = System.IO.File.ReadAllLines(filename);
                for (int i = 0; i < line.Length-1; i++)
                {
                    lines = lines + line[i] + '\n';
                };
                lines += line[line.Length - 1];
                lines += black_box;

                encodeHuffmanCode(lines);
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                filename = dlg.FileName;
            }

            string lines = "";
            if (System.IO.File.Exists(filename))
            {
                string[] line = System.IO.File.ReadAllLines(filename);
                for (int i = 0; i < line.Length - 1; i++)
                {
                    lines = lines + line[i] + '\r' + '\n';
                };
                lines = lines + line[line.Length - 1];
                textBox1.Text = lines;
            };

        }

        private void button4_Click(object sender, EventArgs e)
        {

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "bin files (*.bin)|*.bin";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                filename = dlg.FileName;
            }
            string encoded = "";
            if (System.IO.File.Exists(filename))
            {
                //read binary files
                byte[] filebytes = System.IO.File.ReadAllBytes(filename);
                encoded = string.Concat(filebytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
                textBox1.Text = encoded;

                //open and read frequency file
                string frequencyFile = filename.Split('.')[0] + "_frequency.txt";
                string[] raw_content = System.IO.File.ReadAllLines(frequencyFile);
                string content = raw_content[0];
                for(int i = 1; i < raw_content.Length; i++)
                {
                    content += '\n' + raw_content[i];
                }

                //push frequency to dictionary
                string[] dic_content = content.Split('\u09f0');
                IDictionary<char, int> table = new Dictionary<char, int>();
                for(int i = 0; i < dic_content.Length; i++)
                {
                    char key = dic_content[i][0];
                    int value = Int32.Parse(dic_content[i].Substring(2));
                    table.Add(key, value);
                }

                //build huffman tree
                HuffmanNode root = buildHuffmanTree(table);

                //Iterate in encoded and replace with corresponding character
                string decoded = "";
                string decoded_for_preview = "";
                HuffmanNode currentNode = root;
                foreach (char c in encoded)
                {
                    if (c == '0')
                    {
                        currentNode = currentNode.leftNode;
                    }
                    else
                    {
                        currentNode = currentNode.rightNode;
                    }

                    if (currentNode.leftNode == null && currentNode.rightNode == null)
                    {
                        if (currentNode.character == black_box)
                        {
                            break;
                        }
                        if (currentNode.character == '\n')
                        {
                            decoded_for_preview += '\r';
                        }
                        decoded += currentNode.character;
                        decoded_for_preview += currentNode.character;
                        currentNode = root;
                    }
                }
                textBox2.Text = decoded_for_preview;
            };
        }

        byte[] padding(string encoded)
        {
            var bitsToPad = 8 - encoded.Length % 8;
            if(bitsToPad != 8)
            {
                encoded += new string('0', bitsToPad);
            }
            var numofBytes = encoded.Length / 8;
            byte[] bytes = new byte[numofBytes];

            for(int a = 0;  a < numofBytes; a++)
            {
                bytes[a] = Convert.ToByte(encoded.Substring(a * 8, 8), 2);
            }
            
            return bytes;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(filename))
            {

                if (algorithm == AlgorithmType.HuffmanCoding)
                {
                    SaveFileDialog dlg = new SaveFileDialog();
                    dlg.ShowDialog();
                    filesavename = dlg.FileName;

                    //save binary file
                    FileStream fs = new FileStream(dlg.FileName, FileMode.Create);
                    BinaryWriter bw = new BinaryWriter(fs);
                    string encoded = textBox2.Text;
                    byte[] arr = padding(encoded);
                    bw.Write(arr);
                    bw.Close();

                    //save frequency of character to extention file
                    string new_name = filesavename.Split('.')[0] + "_frequency.txt";
                    FileStream fs_table = new FileStream(new_name, FileMode.Create);
                    string freq = "";
                    foreach (var kvp in frequency_table)
                    {
                        freq += kvp.Key + ":" + kvp.Value.ToString() + '\u09f0';
                    }
                    freq = freq.Remove(freq.Length - 1);
                    StreamWriter sw = new StreamWriter(fs_table);
                    sw.Write(freq);
                    sw.Close();

                    frequency_table.Clear();
                }

                if(algorithm == AlgorithmType.ArithmeticCoding)
                {
                    SaveFileDialog dlg = new SaveFileDialog();
                    dlg.ShowDialog();
                    filesavename = dlg.FileName;

                    //save binary file
                    FileStream fs = new FileStream(dlg.FileName, FileMode.Create);
                    BinaryWriter bw = new BinaryWriter(fs);
                    string encoded = textBox2.Text;
                    byte[] arr = padding(encoded);
                    bw.Write(arr);
                    bw.Close();

                    //save frequency of character to extention file
                    string new_name = filesavename.Split('.')[0] + "_probility.txt";
                    FileStream fs_table = new FileStream(new_name, FileMode.Create);
                    string prob = "";
                    foreach(var kvp in probility_table)
                    {
                        prob += kvp.Key + ":" + kvp.Value.ToString() + '\u09f0';
                    }
                    prob = prob.Remove(prob.Length - 1);
                    StreamWriter sw = new StreamWriter(fs_table);
                    sw.Write(prob);
                    sw.Close();

                    probility_table.Clear();
                }

            }
        }

        string LZW_coding(string a)
        {
            string kq="";
            a = a + ' ';
            int sl = -1;
            for (int i=0;i<a.Length-1;i++)
            {
                bool kt = true;
                for (int j=0;j<=sl;j++)
                    if (tudien_lzw[j]==a[i].ToString())
                    {
                        kt = false;
                        break;
                    };
                if (kt)
                {
                    ++sl;
                    tudien_lzw[sl] =  a[i].ToString();
                };
            };
            string kq1 = sl.ToString() + '\r' + '\n';
            for (int i = 0; i <= sl; i++)
                kq1 = kq1 + tudien_lzw[i].ToString() + '\r' + '\n';
            string s = ""+a[0];
            int i1 = 1;
            int code = 0;
            while (i1<a.Length)
            {
                string c = a[i1].ToString();
                bool kt = true;
                for (int i = 0; i <= sl; i++)
                    if (s + c == tudien_lzw[i])
                    {
                        s = s + c;
                        code = i;
                        kt = false;
                        break;
                    };
                if (kt)
                {
                    kq = kq +code.ToString()+' ';
                    ++sl;
                    tudien_lzw[sl] = s + c;
                    s = c;
                    for (int i = 0; i <= sl; i++)
                        if (s == tudien_lzw[i])
                        {
                            code = i;
                            break;
                        };
                };
                ++i1;
            };
            kq = kq.Remove(kq.Length - 1);
            return kq1+kq;
        }

        void encodeLZW(string data)
        {
            IDictionary<string, int> codeword = new Dictionary<string, int>();
            //initial codeword table
            for (int i = 0; i < 256; i++)
            {
                codeword.Add(char.ToString((char)i), i);
            }

            //initial P and current code (256)
            string P = new string(data[0], 1);
            string encoded = "";
            int current_code = 256;
            
            //processing
            for(int i = 1; i < data.Length; i++)
            {
                char C = data[i];
                if (codeword.ContainsKey(P + C))
                {
                    P += C;
                }
                else
                {
                    encoded += codeword[P]+" ";
                    codeword.Add(P + C, current_code);
                    current_code++;
                    P = char.ToString(C);
                }
            }
            encoded += codeword[P];

            textBox2.Text = encoded;
        }
        private void button5_Click(object sender, EventArgs e)
        {
            algorithm = AlgorithmType.LZWcoding;
            string lines = "";
            if (System.IO.File.Exists(filename))
            {
                string[] line = System.IO.File.ReadAllLines(filename);
                for (int i = 0; i < line.Length - 1; i++)
                {
                    lines = lines + line[i] + '\n';
                };
                lines += line[line.Length - 1];
                encodeLZW(lines);
            };
            
        }
        string LZW_decoding(string a,int sl)
        {
            string kq = "";
            string[] a1 = a.Split(' ');
            for (int i2=0;i2<a1.Length;i2++)
                while (a1[i2].Length<0 && a1[i2][a1[i2].Length-1]==' ')
                {
                    a1[i2]=a1[i2].Remove(a1[i2].Length - 1);
                }
            string s = "";
            int i = 0;
            while (i<a1.Length)
            {
                int k = Int32.Parse(a1[i].ToString());
                string entry = tudien_lzw[k];
                for (int j1 = 0; j1 < entry.Length; j1++)
                    if (entry[j1] == '\n') kq = kq + "\r\n"; else kq = kq + entry[j1];
                if (s!="")
                {
                    ++sl;
                    tudien_lzw[sl] = s + entry[0];
                };
                s = entry;
                ++i;
            };
            return kq;
        }

        void decodeLZW(string data)
        {
            string[] sequences = data.Split(' ');

            //initial codeword table
            IDictionary<string, string> codeword = new Dictionary<string, string>();
            for(int i = 0; i < 256; i++)
            {
                codeword.Add(i.ToString(), char.ToString((char)i));
            }

            //initial Old
            string decoded = "";
            string int_OLD = sequences[0];
            string P = codeword[int_OLD];
            decoded += P;
            int current_code = 256;

            for(int i = 1; i < sequences.Length; i++)
            {
                string int_NEW = sequences[i];
                string C;
                if (codeword.ContainsKey(int_NEW))
                {
                    C = codeword[int_NEW];
                }
                else
                {
                    C = P + P[0];
                }
                decoded += C;
                codeword.Add(current_code.ToString(), P + C[0]);
                current_code++;
                P = C;
            }

            textBox2.Text = decoded;
        }
        private void button7_Click(object sender, EventArgs e)
        {
            /*if (System.IO.File.Exists(filename))
            {
                string[] lines = System.IO.File.ReadAllLines(filename);
                *//*int sl = Int32.Parse(lines[0]);
                int sl1 = -1;
                for (int i = 1; i <lines.Length-1; i++)
                {
                    if (lines[i] != lines[i + 1])
                    {
                        ++sl1;
                        if (lines[i] == "") tudien_lzw[sl1] = "\n"; else  tudien_lzw[sl1] = lines[i];
                    };
                };
                kq = LZW_decoding(lines[lines.Length-1],sl);
                textBox2.Text = kq;*//*
                
            };*/
            //string data = "87 89 83 42 256 71 256 258 262 262 71";
            string data = "87 256 257";
            decodeLZW(data);
        }
        
        string arimetic_coding(string a)
        {
            string kq = "";
            int sl = 0;
            tudien_arimetic[sl].kitu = '$';
            double nen = 0;
            for (int i=0;i<a.Length;++i)
            {
                bool kt = true;
                for (int j=0;j<=sl;j++)
                    if (a[i]==tudien_arimetic[j].kitu)
                    {
                        kt = false;
                        break;
                    };
                if (kt)
                {
                    ++sl;
                    tudien_arimetic[sl].kitu = a[i];
                };
            };
            kq = kq + sl.ToString();
            double kc = 1 / ((double)sl+1);
            tudien_arimetic[0].high = kc;
            kq = kq + '\r' + '\n' + tudien_arimetic[0].low + ' ' + tudien_arimetic[0].high + ' ' + tudien_arimetic[0].kitu;
            for (int i=1;i<=sl;i++)
            {
                tudien_arimetic[i].low = tudien_arimetic[i - 1].high;
                tudien_arimetic[i].high = tudien_arimetic[i].low + kc;
                kq = kq + '\r' + '\n' + tudien_arimetic[i].low + ' ' + tudien_arimetic[i].high + ' ' + tudien_arimetic[i].kitu;
            };
            kq = kq + '\r' + '\n';
            a = a + '$';
            for (int i=0;i<a.Length;i++)
            {
                int j = 0;
                while (a[i] != tudien_arimetic[j].kitu) ++j;
                nen = (tudien_arimetic[j].high - tudien_arimetic[j].low) / 2 + tudien_arimetic[j].low;
                double kc1 = tudien_arimetic[j].high - tudien_arimetic[j].low;
                tudien_arimetic[0].high = tudien_arimetic[j].low + kc1 * (tudien_arimetic[0].high - tudien_arimetic[0].low);
                tudien_arimetic[0].low = tudien_arimetic[j].low;
                for (int i1=1;i1<=sl;i1++)
                {
                    double tam = tudien_arimetic[i1].high - tudien_arimetic[i1].low;
                    tudien_arimetic[i1].low = tudien_arimetic[i1 - 1].high;
                    tudien_arimetic[i1].high = tudien_arimetic[i1].low + tam * kc1;
                };
            };
            kq = kq + nen.ToString();
            return kq;
        }

        double fractionBinarytoDecimal(string bin)
        {
            if(bin == "")
            {
                return 0.0;
            }
            double answer = 0.0, powerofTwo = 2.0;
            for(int i = 0; i < bin.Length; i++)
            {
                answer += (bin[i]-'0')/powerofTwo;
                powerofTwo *= 2.0;
            }

            return answer;
        }
        IDictionary<char, double> probality_gathering(string data)
        {
            int total = data.Length;
            IDictionary<char, double> table = new Dictionary<char, double>();
            foreach (char c in data)
            {
                if (table.ContainsKey(c))
                {
                    table[c] = table[c] + (double)1 / total;
                }
                else
                {
                    table.Add(c, (double)1 / total);
                }
            }
            return table;
        }

        string generating_codeword(double low, double high)
        {
            string codeword = "";
            int k = 0;
            while(fractionBinarytoDecimal(codeword) < low)
            {
                codeword += '1';
                if(fractionBinarytoDecimal(codeword) > high)
                {
                    codeword = codeword.Remove(k)+'0';
                }
                k++;
            }
            return codeword;
        }

        void encodeArithmetic(string data)
        {
            //frequency gathering into probality
            IDictionary<char, double> table = probality_gathering(data);
            probility_table = table;

            //set up low, high for each character
            IDictionary<char, double> range_low = new Dictionary<char, double>();
            IDictionary<char, double> range_high = new Dictionary<char, double>();
            double current_low = 0;
            string prob_distribution = "";
            foreach (var kvp in table)
            {
                range_low.Add(kvp.Key, current_low);
                range_high.Add(kvp.Key, current_low + kvp.Value);
                current_low += kvp.Value;
                prob_distribution += string.Format("symbol: {0}, {1}, [{2},{3})\r\n", kvp.Key, kvp.Value, range_low[kvp.Key], range_high[kvp.Key]);
            }
            textBox3.Text = prob_distribution;

            //just for testing;
            //data = "caee\u25a0";

            //Arithmetic on the go and get low and high of entire string
            int size = data.Length;
            double low = 0.0, high = 1.0, range = 1.0;
            string test = string.Format("Begin: {0}  [{1},{2}), {3}\r\n", ' ', low, high, range);
            for(int i = 0; i < size; i++)
            {
                char c = data[i];
                high = low + range * range_high[c];
                low = low + range * range_low[c];
                range = high - low;
                test += string.Format("symbol: {0}, [{1},{2}), {3}\r\n", c, low, high, range);
            }
            Console.WriteLine(test);

            //Generating codeword
            string encoded = generating_codeword(low, high);

            textBox2.Text = encoded;
        }
        private void button6_Click(object sender, EventArgs e)
        {
            algorithm = AlgorithmType.ArithmeticCoding;
            string lines = "";
            if (System.IO.File.Exists(filename))
            {
                string[] line = System.IO.File.ReadAllLines(filename);
                for (int i = 0; i < line.Length - 1; i++)
                {
                    lines = lines + line[i] + '\n';
                };
                lines += line[line.Length - 1];
                lines += black_box;
                
                encodeArithmetic(lines);
            };
        }
        string arimetic_decoding(string a,int sl)
        {
            string kq = "";
            double nen = Double.Parse(a);
            while (kq=="" || kq[kq.Length-1]!='$')
            {
                int j = 0;
                while (tudien_arimetic[j].high <= nen)
                {
                    ++j;
                };
                kq = kq + tudien_arimetic[j].kitu;
                double kc1 = tudien_arimetic[j].high - tudien_arimetic[j].low;
                tudien_arimetic[0].high = tudien_arimetic[j].low + kc1 * (tudien_arimetic[0].high - tudien_arimetic[0].low);
                tudien_arimetic[0].low = tudien_arimetic[j].low;
                for (int i1 = 1; i1 <= sl; i1++)
                {
                    double tam = tudien_arimetic[i1].high - tudien_arimetic[i1].low;
                    tudien_arimetic[i1].low = tudien_arimetic[i1 - 1].high;
                    tudien_arimetic[i1].high = tudien_arimetic[i1].low + tam * kc1;
                };

            };
            kq = kq.Remove(kq.Length - 1);
            return kq;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "bin files (*.bin)|*.bin";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                filename = dlg.FileName;
            }
            string encoded = "";
            if (System.IO.File.Exists(filename))
            {
                //read binary files
                byte[] filebytes = System.IO.File.ReadAllBytes(filename);
                encoded = string.Concat(filebytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
                textBox1.Text = encoded;

                //open and read probility file
                string probilityFile = filename.Split('.')[0] + "_probility.txt";
                string[] raw_content = System.IO.File.ReadAllLines(probilityFile);
                string content = raw_content[0];
                for (int i = 1; i < raw_content.Length; i++)
                {
                    content += '\n' + raw_content[i];
                }

                //push probility to dictionary
                string[] dic_content = content.Split('\u09f0');
                IDictionary<char, double> table = new Dictionary<char, double>();
                for (int i = 0; i < dic_content.Length; i++)
                {
                    char key = dic_content[i][0];
                    double val = double.Parse(dic_content[i].Substring(2));
                    table.Add(key, val);
                }

                //set up low, high for each character
                IDictionary<char, double> range_low = new Dictionary<char, double>();
                IDictionary<char, double> range_high = new Dictionary<char, double>();
                double current_low = 0;
                string prob_distribution = "";
                foreach (var kvp in table)
                {
                    range_low.Add(kvp.Key, current_low);
                    range_high.Add(kvp.Key, current_low + kvp.Value);
                    current_low += kvp.Value;
                    prob_distribution += string.Format("symbol: {0}, {1}, [{2},{3})\r\n", kvp.Key, kvp.Value, range_low[kvp.Key], range_high[kvp.Key]);
                }
                textBox3.Text = prob_distribution;


                //get fraction binary code and convert to decimal
                double value = fractionBinarytoDecimal(encoded);
                Console.WriteLine("begin value: {0}", value);

                //decode on the go
                double low, high, range;
                string decoded = "";
                string decoded_for_preview = "";
                char s = '\0';
                do {
                    foreach(var kvp in range_high)
                    {
                        if (range_low[kvp.Key] <= value && value < kvp.Value)
                        {
                            s = kvp.Key;
                            break;
                        }
                    }
                    if (s == '\n')
                        decoded_for_preview += '\r';
                    decoded_for_preview += s;
                    decoded += s;
                    low = range_low[s];
                    high = range_high[s];
                    range = high - low;
                    value = (value - low) / range;
                } while (s != black_box);
                decoded = decoded.Remove(decoded.Length - 1);
                textBox2.Text = decoded_for_preview;
            };
        }

        private void button9_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "bin files (*.bin)|*.bin";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                filename = dlg.FileName;
            }
            if (System.IO.File.Exists(filename))
            {
                byte[] filebytes = System.IO.File.ReadAllBytes(filename);
                var result = string.Concat(filebytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
                textBox1.Text = result;
            };
        }
    }
}
