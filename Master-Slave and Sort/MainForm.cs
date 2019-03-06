using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Master_Slave_and_Sort
{
    public partial class MainForm : Form
    {
        Random random = new Random();
        int length, loop;
        private int[] winners;

        public MainForm()
        {
            InitializeComponent();
        }

        private int[] randomArray(int length)
        {
            int[] array = new int[length];
            for (int i = 0; i < length; i++)
            {
                int x = random.Next(length);
                array[i] = x;
            }
            return array;
        }

        private int[] cloneArray(int[] origin)
        {
            int[] array = new int[origin.Length];
            for (int i = 0; i < origin.Length; i++)
            {
                array[i] = origin[i];
            }
            return array;
        }

        private void swap(ref int a, ref int b)
        {
            int t = a;
            a = b;
            b = t;
        }

        private void selectionSort(ref int[] array)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                for (int j = i + 1; j < array.Length; j++)
                {
                    if (array[i] > array[j])
                    {
                        swap(ref array[i], ref array[j]);
                    }
                }
            }
        }

        private void quickSort(ref int[] array)
        {
            quickSort(ref array, 0, array.Length - 1);
        }

        private void quickSort(ref int[] array, int left, int right)
        {
            if (left > right || left < 0 || right < 0) return;

            int index = partition(ref array, left, right);

            if (index != -1)
            {
                quickSort(ref array, left, index - 1);
                quickSort(ref array, index + 1, right);
            }
        }

        private int partition(ref int[] array, int left, int right)
        {
            if (left > right) return -1;
            int end = left;
            int pivot = array[right];
            for (int i = left; i < right; i++)
            {
                if (array[i] < pivot)
                {
                    swap(ref array[i], ref array[end]);
                    end++;
                }
            }
            swap(ref array[end], ref array[right]);
            return end;
        }

        private void heapSort(ref int[] array)
        {
            for (int i = array.Length / 2 - 1; i >= 0; i--)
                heaping(ref array, array.Length, i);
            for (int i = array.Length - 1; i >= 0; i--)
            {
                swap(ref array[0], ref array[i]);
                heaping(ref array, i, 0);
            }
        }

        private void heaping(ref int[] array, int n, int i)
        {
            int largest = i;
            int left = 2 * i + 1;
            int right = 2 * i + 2;
            if (left < n && array[left] > array[largest])
                largest = left;
            if (right < n && array[right] > array[largest])
                largest = right;
            if (largest != i)
            {
                int swap = array[i];
                array[i] = array[largest];
                array[largest] = swap;
                heaping(ref array, n, largest);
            }
        }
        
        private void win(int i, int algorithm)
        {
            if (winners[i] == 0)
            {
                winners[i] = algorithm;
                Invoke(new Action(() => updateProgress(i, algorithm)));
                if (i < loop - 1)
                {
                    race(++i);
                } else
                {
                    Invoke(new Action(showResult));
                }
            }
        }

        private void updateProgress(int i, int algorithm)
        {
            progressBar.Value = i + 1;
            switch (algorithm)
            {
                case 1:
                    Invoke(new Action(() => selectionProgress.Value++));
                    break;

                case 2:
                    Invoke(new Action(() => heapProgress.Value++));
                    break;

                case 3:
                    Invoke(new Action(() => quickProgress.Value++));
                    break;
            }
        }

        private void showResult()
        {
            int selection = winners.Count(w => w == 1);
            int heap = winners.Count(w => w == 2);
            int quick = winners.Count(w => w == 3);
            resultLabel.Text = $"Selection sort win {selection} times, Heap sort win {heap} times, Quick sort win {quick} times";
        }

        private void race(int i)
        {
            int[] array1 = randomArray(length);
            int[] array2 = cloneArray(array1);
            int[] array3 = cloneArray(array1);

            Thread selection = new Thread(() =>
            {
                selectionSort(ref array1);
                win(i, 1);
            });
            Thread heap = new Thread(() =>
            {
                heapSort(ref array2);
                win(i, 2);
            });
            Thread quick = new Thread(() =>
            {
                quickSort(ref array3);
                win(i, 3);
            });

            selection.Start();
            if (i % 2 == 0)
            {
                heap.Start();
                quick.Start();
            } else
            {
                quick.Start();
                heap.Start();
            }

        }

        private void initProgressBars()
        {
            progressBar.Maximum = loop;
            selectionProgress.Maximum = loop;
            heapProgress.Maximum = loop;
            quickProgress.Maximum = loop;

            progressBar.Value = 0;
            selectionProgress.Value = 0;
            heapProgress.Value = 0;
            quickProgress.Value = 0;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(lengthTextBox.Text, out length))
            {
                length = 10000;
            }
            if (!int.TryParse(loopTextBox.Text, out loop))
            {
                loop = 100;
            }
            resultLabel.Text = "";
            initProgressBars();
            winners = new int[loop];
            new Thread(() => race(0)).Start();
        }
    }
}
