using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.VisualBasic;

namespace NoyauCommun
{
    namespace Erreurs
    {
        public class GestionErreurs
        {
            private const string NOM_FICHIER_LOG = "Rummy.log";

            private static string CheminFichierLog
            {
                get
                {

                    return Path.Combine(new FileInfo(Application.ExecutablePath.Replace("/", "\\")).Directory.FullName,
                                        NOM_FICHIER_LOG);
                }
            }

            public static void EnregistrerErreur(Exception e, string texte = "")
            {
                StreamWriter FileWriter = null;

                try
                {
                    FileWriter = new StreamWriter(CheminFichierLog, true);

                    DateTime dt = DateTime.Now;
                    FileWriter.Write(string.Format("{0}/{1}/{2} ",
                                                   dt.Date.Year.ToString(),
                                                   dt.Date.Month.ToString("00"),
                                                   dt.Date.Day.ToString("00")));

                    FileWriter.Write(string.Format("{0}:{1}:{2} -> ",
                                                   dt.TimeOfDay.Hours.ToString("00"),
                                                   dt.TimeOfDay.Minutes.ToString("00"),
                                                   dt.TimeOfDay.Seconds.ToString("00")));

                    FileWriter.Write(string.Format("{0} ({1})",
                                                   texte,
                                                   e.Message));

                    foreach (string tmp in e.StackTrace.Split(Environment.NewLine.ToCharArray()))
                        FileWriter.WriteLine(tmp.Trim());

                    FileWriter.WriteLine();
                }
                catch (IOException)
                {
                }
                catch (Exception)
                {
                }
                finally
                {
                    if (FileWriter != null)
                    {
                        FileWriter.Flush();
                        FileWriter.Close();
                    }
                }
            }

            public static void Tracer(string texte = "", bool separateur = false)
            {
                StreamWriter FileWriter = null;

                try
                {
                    FileWriter = new StreamWriter(CheminFichierLog, true);

                    if (separateur)
                        FileWriter.WriteLine(new String('-', 50));

                    if (texte.Length > 0)
                        FileWriter.WriteLine(texte);

                    FileWriter.WriteLine();
                }
                catch (IOException)
                {
                }
                catch (Exception)
                {
                }
                finally
                {
                    if (FileWriter != null)
                    {
                        FileWriter.Flush();
                        FileWriter.Close();
                    }
                }
            }

            public static void SupprimerFichierLog()
            {
                try
                {
                    string strNomFichierLog = Path.Combine(Application.ExecutablePath, NOM_FICHIER_LOG);
                    if (File.Exists(strNomFichierLog))
                        File.Delete(strNomFichierLog);
                }
                catch
                {
                }
            }
        }
    }
}