using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ComparadorIdeas
{
    public static class AlgoritmoSimilitud
    {
        public static double CalculateJaccardSimilarity(HashSet<string> words1, HashSet<string> words2)
        {
            if (words1.Count == 0 && words2.Count == 0) return 1.0;

            var intersection = words1.Intersect(words2).Count();
            var union = words1.Union(words2).Count();

            return (double)intersection / union;
        }

        public static List<ResultadoSimilitud> CompararIdeas(List<Idea> ideas)
        {
            var similitudes = new List<ResultadoSimilitud>();

            for (int i = 0; i < ideas.Count; i++)
            {
                for (int j = i + 1; j < ideas.Count; j++)
                {
                    double similarity = CalculateJaccardSimilarity(ideas[i].Words, ideas[j].Words);
                    similitudes.Add(new ResultadoSimilitud(ideas[i], ideas[j], similarity));
                }
            }

            return similitudes.OrderByDescending(s => s.Similitud).ToList();
        }

        public static List<ResultadoSimilitud> ObtenerParesExclusivos(List<ResultadoSimilitud> similitudes)
        {
            // ALGORITMO DE EMPAREJAMIENTO EXCLUSIVO (Greedy Matching)
            var ideasEmparejadas = new HashSet<int>();
            var paresFinales = new List<ResultadoSimilitud>();

            foreach (var sim in similitudes)
            {
                // Si ninguna de las dos ideas ha sido emparejada todavía
                if (!ideasEmparejadas.Contains(sim.Idea1.Id) && !ideasEmparejadas.Contains(sim.Idea2.Id))
                {
                    paresFinales.Add(sim);
                    ideasEmparejadas.Add(sim.Idea1.Id);
                    ideasEmparejadas.Add(sim.Idea2.Id);
                }
            }

            return paresFinales;
        }
    }

    public class ResultadoSimilitud
    {
        public Idea Idea1 { get; }
        public Idea Idea2 { get; }
        public double Similitud { get; }

        public ResultadoSimilitud(Idea i1, Idea i2, double sim)
        {
            Idea1 = i1;
            Idea2 = i2;
            Similitud = sim;
        }
    }

    public class Idea
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public HashSet<string> Words { get; set; }

        private static readonly HashSet<string> StopWords = new HashSet<string> 
        { 
            "de", "la", "que", "el", "en", "y", "a", "los", "del", "se", "las", "por", "un", 
            "para", "con", "no", "una", "su", "al", "es", "o", "mi", "mis", "son", "sin", "sean", "lo", "mas"
        };

        public Idea(int id, string text)
        {
            Id = id;
            Text = text;
            
            var cleanText = Regex.Replace(text.ToLower(), @"[^\w\s]", "");
            var wordsArray = cleanText.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            
            Words = new HashSet<string>();

            foreach (var w in wordsArray)
            {
                if (StopWords.Contains(w)) continue;

                var normalizedWord = w;
                
                // Stemming muy básico
                if (normalizedWord.EndsWith("es") && normalizedWord.Length > 4) 
                    normalizedWord = normalizedWord.Substring(0, normalizedWord.Length - 2);
                else if (normalizedWord.EndsWith("s") && normalizedWord.Length > 3)
                    normalizedWord = normalizedWord.Substring(0, normalizedWord.Length - 1);

                // HOMOLOGACIÓN DE CONCEPTOS (Sinónimos del dominio musical/software)
                if (normalizedWord == "tema") normalizedWord = "cancion"; 
                
                // Acciones de reproducir/escuchar
                if (normalizedWord == "reproducida" || normalizedWord == "reproduccion" || 
                    normalizedWord == "escuchada" || normalizedWord == "escuche" || 
                    normalizedWord == "escucho" || normalizedWord == "escucha") 
                {
                    normalizedWord = "escuchar";
                }

                // Acciones de ver/mostrar
                if (normalizedWord == "ver" || normalizedWord == "verla" || normalizedWord == "listar")
                {
                    normalizedWord = "mostrar";
                }

                // Acciones de guardar/crear
                if (normalizedWord == "agregada" || normalizedWord == "agregar" || normalizedWord == "guardar")
                {
                    normalizedWord = "crear";
                }

                // Conceptos de tiempo
                if (normalizedWord == "hora" || normalizedWord == "contador")
                {
                    normalizedWord = "tiempo";
                }

                // Grupos musicales
                if (normalizedWord == "artista")
                {
                    normalizedWord = "banda";
                }

                // "favorito", "favorita", "favoritos" -> favorit
                if (normalizedWord.StartsWith("favorit"))
                {
                    normalizedWord = "favorito";
                }

                if (normalizedWord == "ultima")
                {
                    normalizedWord = "ultimo";
                }

                Words.Add(normalizedWord);
            }
        }
    }
}
