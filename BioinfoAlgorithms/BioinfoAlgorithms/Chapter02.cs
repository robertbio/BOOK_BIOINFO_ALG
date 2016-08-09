﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace BioinfoAlgorithms
{
    class RunChapter02:Chapter02
    {
        public RunChapter02(string excercise)
        {
            Chapter02 chapter = new Chapter02();

            List<string> dnaStrings;
            int k;
            int d;
            switch (excercise)
            {
                case "2A":
                    dnaStrings = new List<string>
                    {
                        "AAAAAAAAATTTAAAAAAA",
                        "AAAAATATTTAAAAAA",
                        "AAAAATTTTAAAAA",
                    };
                    k = 4;
                    d = 0;
                    Console.WriteLine(string.Join("\n", chapter.MotifEnumerator(dnaStrings, k, d)));
                    Console.ReadLine();
                    break;
                case "2B":
                    dnaStrings = new List<string>
                    {
                        "TCTCTCTCTC",
                        "TGTGTGTGTG",
                        "TTTTTTTTTT",
                    };
                    k = 5;
                    Console.WriteLine(chapter.MedianString(dnaStrings, k));
                    Console.ReadLine();
                    break;
                case "2C":
                    // List<ProfileMatrixEntry> profileMatrix = new List<ProfileMatrixEntry>();
                    // profileMatrix.Add(new ProfileMatrixEntry {Base = Alphabet[0], Pos = 0, Prob = 0.2});
                    // profileMatrix.Add(new ProfileMatrixEntry {Base = Alphabet[1], Pos = 0, Prob = 0.1});
                    // profileMatrix.Add(new ProfileMatrixEntry {Base = Alphabet[2], Pos = 0, Prob = 0.0});
                    // profileMatrix.Add(new ProfileMatrixEntry {Base = Alphabet[3], Pos = 0, Prob = 0.7});

                    // profileMatrix.Add(new ProfileMatrixEntry {Base = Alphabet[0], Pos = 1, Prob = 0.2});
                    // profileMatrix.Add(new ProfileMatrixEntry {Base = Alphabet[1], Pos = 1, Prob = 0.6});
                    // profileMatrix.Add(new ProfileMatrixEntry {Base = Alphabet[2], Pos = 1, Prob = 0.0});
                    // profileMatrix.Add(new ProfileMatrixEntry {Base = Alphabet[3], Pos = 1, Prob = 0.2});

                    // profileMatrix.Add(new ProfileMatrixEntry {Base = Alphabet[0], Pos = 2, Prob = 0.0});
                    // profileMatrix.Add(new ProfileMatrixEntry {Base = Alphabet[1], Pos = 2, Prob = 0.0});
                    // profileMatrix.Add(new ProfileMatrixEntry {Base = Alphabet[2], Pos = 2, Prob = 1.0});
                    // profileMatrix.Add(new ProfileMatrixEntry {Base = Alphabet[3], Pos = 2, Prob = 0.0});

                    // profileMatrix.Add(new ProfileMatrixEntry {Base = Alphabet[0], Pos = 3, Prob = 0.0});
                    // profileMatrix.Add(new ProfileMatrixEntry {Base = Alphabet[1], Pos = 3, Prob = 0.0});
                    // profileMatrix.Add(new ProfileMatrixEntry {Base = Alphabet[2], Pos = 3, Prob = 1.0});
                    // profileMatrix.Add(new ProfileMatrixEntry {Base = Alphabet[3], Pos = 3, Prob = 0.0});

                    // k = 4;

                    // int [] entry = new int[2];
                    // entry[0] = 3;
                    // entry[1] = 0;
                    // string dna = "AAATTTTCGGAA";
                    // Console.WriteLine(chapter.MostProbableKmer(dna, k, profileMatrix));
                    // Console.ReadLine();

                    List<string> dnas = new List<string> {"ATGC",
                                                          "TACG",
                                                          "GGGG",
                                                          "CAAA" };
                    var pm = chapter.DnaToProfileMatrix(dnas);
                    chapter.PrintPm(pm);
                    break;
                case "2H":
                    dnaStrings = new List<string>
                    {
                        "AAAAAAAAATTTAAAAAAA",
                        "AAAAATATTTAAAAAAGCG",
                        "AAAAATTTTAAAAAGGATT",
                    };
                    string pattern = "AAAAG";
                    Console.WriteLine(chapter.DistanceBetweenPatternAndStrings(pattern, dnaStrings).ToString());
                    Console.ReadLine();
                    break;
                default:
                    Console.Write("Cannot interpret exercise number");
                    Console.ReadLine();
                    break;
            }
        }
    }

    class Chapter02:Chapter01
    {
        public List<ProfileMatrixEntry> DnaToProfileMatrix(List<string> dnaStrings)
        {
            List<ProfileMatrixEntry> pm = new List<ProfileMatrixEntry>();

            for(int i = 0; i < dnaStrings.First().Length; i++)
            {
                List<string> sequence = new List<string>();

                Dictionary<string, int> ntPosition = new Dictionary<string, int>();
                foreach (string nt in Alphabet) {
                    ntPosition[nt] = 0;
                }

                for (int j = 0; j < dnaStrings.Count; j++)
                {
                    if (dnaStrings[j].Length != dnaStrings.First().Length)
                    {
                        Console.WriteLine("DNA strings are of unequal length!");
                        Environment.Exit(1); 
                    }
                
                    string currentDna = dnaStrings[j];
                    string currentNt = currentDna[i].ToString();
                    ntPosition[currentNt] += 1;
                }

                foreach (string nt in Alphabet)
                {
                    double prob = ntPosition[nt]/(double)dnaStrings.Count;
                    pm.Add(new ProfileMatrixEntry {Base = nt, Pos = i, Prob = prob});
                }
            }

            return pm;
        }

        public void PrintPm(List<ProfileMatrixEntry> profileMatrix)
        {
            int PmLength = profileMatrix.Count/Alphabet.Count;
            foreach (string nt in Alphabet)
            {
                Console.Write(nt);
                for (int i = 0; i < PmLength; i++)
                {
                    IEnumerable<double> entries = from a in profileMatrix
                                                  where a.Base == nt
                                                  where a.Pos == i
                                                  select a.Prob;

                    Console.Write("\t" + entries.First().ToString());
                }
                Console.WriteLine();
            }
            Console.ReadLine();
        }

        public string MostProbableKmer( string text, int k, List<ProfileMatrixEntry> profileMatrix )
        {
            string pattern = "";
            double prob = 0.0;

            List<int[]> windows = StringSlidingWindows(text, k);

            foreach (int[] window in windows)
            {
                string currentPattern = text.Substring(window[0], k);
                double currentProb = KmerProbabilityFromPm(profileMatrix, currentPattern);
                // Console.WriteLine(currentProb.ToString());
                if (currentProb >= prob)
                {
                    pattern = currentPattern;
                    prob = currentProb;
                }
            }

            return pattern;
        }

        public double KmerProbabilityFromPm(List<ProfileMatrixEntry> pm, string pattern)
        {
            double prob = 1.0;
            for (int i = 0; i < pattern.Length; i++)
            {
                var i1 = i;
                var i2 = i;
                IEnumerable<double> currentProbs = from a in pm
                    where a.Base == pattern[i1].ToString()
                    where a.Pos == i2
                    select a.Prob;

                prob *= currentProbs.First();
            }

            return prob;
        }

        public string MedianString(List<string> dnaStrings, int k)
        {
            string median = "";

            int distance = 1000000;
            for (int i = 0; i < Math.Pow(EnumUtil.GetValues<BioinfoAlgorithms.AlphabetEnum>().Count(), k) ; i++)
            {
                string pattern = NumberToPattern(i, k);
                int currentDist = DistanceBetweenPatternAndStrings(pattern, dnaStrings);
                if (distance > currentDist)
                {
                    distance = currentDist;
                    median = pattern;
                }

            }

            return median;
        }
        public int DistanceBetweenPatternAndStrings(string pattern, List<string> dnaStrings)
        {
            int k = pattern.Length;
            int distance = 0;

            foreach (string text in dnaStrings)
            {
                int hammingDistance = 1000000;
                List<int[]> windows = StringSlidingWindows(text, k);
                foreach (int[] window in windows)
                {
                    string patternP = text.Substring(window[0], k);
                    int latestHammingDist = HammingDistance(pattern, patternP); 
                    if (hammingDistance > latestHammingDist)
                    {
                        hammingDistance = latestHammingDist;
                    }
                }

                distance += hammingDistance;
            }

            return distance;
        }

        public List<string> MotifEnumerator(List<string> dnaStrings, int k, int d)
        {   
            var patternsDict = new MyListDictionary();

            foreach (string dna in dnaStrings)
            {
                List<int[]> windows = StringSlidingWindows(dna, k);
                foreach (int[] window in windows)
                {
                    string pattern = dna.Substring(window[0], k);
                    List<string> neighbors = Neighbors(pattern, d);
                    foreach (string neighbor in neighbors)
                    {
                        foreach (string dnaP in dnaStrings)
                        {
                            List<int[]> windowsP = StringSlidingWindows(dnaP, k);
                            foreach (int[] windowP in windowsP)
                            {
                                string patternP = dnaP.Substring(windowP[0], k);
                                if (neighbor == patternP)
                                {
                                    patternsDict.Add(pattern,dnaP);
                                }
                            }
                        } 
                    } 
                }
            }

            var patternsList = new List<string>();
            var patterns = new List<string>(patternsDict.InternalDictionary.Keys);
            foreach (string pattern in patterns)
            {
                if (patternsDict.InternalDictionary[pattern].Count == dnaStrings.Count)
                {
                   patternsList.Add(pattern); 
                }
                
            }

            return patternsList;
        }


        public List<int[]> StringSlidingWindows(string foo, int k)
        {
            List<int[]> windows = new List<int[]>();
            for (int i = 0; i <= foo.Length - k; i++)
            {
                int[] coords = new int[2];
                coords[0] = i;
                coords[1] = i + k;
                windows.Add(coords);
            }
            return windows;
        }
    }
}
