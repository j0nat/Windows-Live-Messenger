using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Text.RegularExpressions;

using WLMClient.Layout;
using WLMClient.Resource.Images;

namespace WLMClient.UI.Data
{
    class TextParser
    {
        public static void ParseText(FrameworkElement element, bool findLinks)
        {
            TextBlock textBlock = null;
            RichTextBox textBox = element as RichTextBox;

            if (textBox == null)
                textBlock = element as TextBlock;

            if (textBox == null && textBlock == null)
                return;

            if (textBox != null)
            {
                FlowDocument doc = textBox.Document;
                for (int blockIndex = 0; blockIndex < doc.Blocks.Count; blockIndex++)
                {
                    Block b = doc.Blocks.ElementAt(blockIndex);
                    Paragraph p = b as Paragraph;
                    if (p != null)
                    {
                        ProcessInlines(textBox, p.Inlines, findLinks);
                    }
                }
            }
            else
            {
                ProcessInlines(null, textBlock.Inlines, findLinks);
            }
        }

        private static int FindFirstEmoticon(string text, int startIndex, out string emoticonFound)
        {
            emoticonFound = string.Empty;
            int minIndex = -1;

            foreach (string emoticon in Emoticons.INDEX_IN_IMAGE.Keys)
            {
                int index = text.IndexOf(emoticon.ToLower(), startIndex);
                if (index >= 0)
                {
                    if (minIndex < 0 || index < minIndex)
                    {
                        minIndex = index;

                        if (text.Contains(emoticon.ToLower()))
                        {
                            emoticonFound = emoticon.ToLower();
                        }

                        if (text.Contains(emoticon.ToUpper()))
                        {
                            emoticonFound = emoticon.ToUpper();
                        }
                    }
                }
                else
                {
                    index = text.IndexOf(emoticon.ToUpper(), startIndex);

                    if (index >= 0)
                    {
                        if (minIndex < 0 || index < minIndex)
                        {
                            minIndex = index;

                            if (text.Contains(emoticon.ToLower()))
                            {
                                emoticonFound = emoticon.ToLower();
                            }

                            if (text.Contains(emoticon.ToUpper()))
                            {
                                emoticonFound = emoticon.ToUpper();
                            }
                        }
                    }
                }


            }

            return minIndex;
        }

        public static string GetPlainText(FlowDocument doc)
        {
            StringBuilder result = new StringBuilder();

            foreach (Block b in doc.Blocks)
            {
                Paragraph p = b as Paragraph;
                if (p != null)
                {
                    foreach (Inline i in p.Inlines)
                    {
                        if (i is Run)
                        {
                            Run r = i as Run;

                            result.Append(r.Text);
                        }
                        else if (i is InlineUIContainer)
                        {
                            InlineUIContainer ic = i as InlineUIContainer;
                            if (ic.Child is Image)
                            {
                                result.Append((ic.Child as Image).Tag.ToString());
                            }
                        }
                    }
                }

                result.Append(Environment.NewLine);
            }

            return result.ToString();
        }


        public static List<string> GetLinks(string message)
        {
            List<string> list = new List<string>();
            Regex urlRx = new Regex(@"((https?|ftp|file)\://|www.)[A-Za-z0-9\.\-]+(/[A-Za-z0-9\?\&\=;\+!'\(\)\*\-\._~%]*)*", RegexOptions.IgnoreCase);

            MatchCollection matches = urlRx.Matches(message);
            foreach (Match match in matches)
            {
                list.Add(match.Value);
            }
            return list;
        }

        public static void ProcessInlines(RichTextBox textBox, InlineCollection inlines, bool findLinks)
        {
            for (int inlineIndex = 0; inlineIndex < inlines.Count; inlineIndex++)
            {
                Inline i = inlines.ElementAt(inlineIndex);
                if (i is Run)
                {
                    Run r = i as Run;
                    string text = r.Text;

                    if (findLinks)
                    {
                        ProcessParagraphLink(textBox, text, i);
                    }

                    string emoticonFound = string.Empty;

                    int index = FindFirstEmoticon(text, 0, out emoticonFound);
                    if (index >= 0)
                    {
                        TextPointer tp = i.ContentStart;

                        bool reposition = false;
                        while (!tp.GetTextInRun(LogicalDirection.Forward).StartsWith(emoticonFound))
                        {
                            tp = tp.GetNextInsertionPosition(LogicalDirection.Forward);
                        }

                        TextPointer end = tp;
                        for (int j = 0; j < emoticonFound.Length; j++)
                        {
                            end = end.GetNextInsertionPosition(LogicalDirection.Forward);
                        }

                        TextRange tr = new TextRange(tp, end);
                        if (textBox != null)
                        {
                            reposition = textBox.CaretPosition.CompareTo(tr.End) == 0;
                        }

                        tr.Text = string.Empty;

                        Image image = new Image();

                        image.SnapsToDevicePixels = true;
                        image.Source = LoadResource.GetEmoticon(emoticonFound.ToLower());
                        image.Width = 19;
                        image.Height = 19;
                        image.Stretch = Stretch.Fill;
                        image.Tag = emoticonFound.ToLower();

                        RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
                        RenderOptions.SetEdgeMode(image, EdgeMode.Aliased);

                        InlineUIContainer iui = new InlineUIContainer(image, tp);
                        iui.BaselineAlignment = BaselineAlignment.TextBottom;

                        if (textBox != null && reposition)
                        {
                            textBox.CaretPosition = tp.GetNextInsertionPosition(LogicalDirection.Forward);
                        }
                    }
                }
            }
        }

        public static void ProcessParagraphLink(RichTextBox textBox, string text, Inline i)
        {
            foreach (string str in GetLinks(text))
            {
                TextPointer tp = i.ContentStart;
                bool reposition = false;
                while (!tp.GetTextInRun(LogicalDirection.Forward).StartsWith(str))
                    tp = tp.GetNextInsertionPosition(LogicalDirection.Forward);
                TextPointer end = tp;
                for (int j = 0; j < str.Length; j++)
                    end = end.GetNextInsertionPosition(LogicalDirection.Forward);
                TextRange tr = new TextRange(tp, end);
                if (textBox != null)
                    reposition = textBox.CaretPosition.CompareTo(tr.End) == 0;
                tr.Text = string.Empty;


                TextBlock block = new TextBlock();
                block.Text = str;
                block.ForceCursor = true;
                block.Cursor = System.Windows.Input.Cursors.Hand;
                block.TextDecorations = TextDecorations.Underline;
                block.Foreground = Brushes.Blue;
                block.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(block_PreviewMouseLeftButtonDown);

                InlineUIContainer iui = new InlineUIContainer(block, tp);
            }
        }

        static void block_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string url = ((TextBlock)sender).Text;
            System.Diagnostics.Process.Start(url);
        }
    }
}
