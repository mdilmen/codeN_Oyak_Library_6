// Decompiled with JetBrains decompiler
// Type: codeN_JobExecuter.Service
// Assembly: codeN_Oyak_Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89EBF006-92A9-443A-8BC3-6147E9F59106
// Assembly location: C:\Users\OYAKUSER\Desktop\codeN_Oyak_Library_4.dll

using codeN;
using eBAFlowScrAdp;
using eBAFlowScrAdp.Objects;
using eBAPI.Connection;
using eBAPI.DocumentManagement;
using eBAPI.Workflow;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace codeN_JobExecuter
{
  public class Service : IDisposable
  {
    private BaseFlowCode baseFlow;
    private FlowVariable _vrFaturaID;
    private FlowVariable vrZarfID;
    private FlowVariable gelisZamani;
    private FlowVariable sonHatirlatma;
    private FlowVariable hatirlatmaAktif;
    private FlowPauser AkisDurdurucu1;

    public Service(BaseFlowCode remoteBaseFlow)
    {
      this.baseFlow = remoteBaseFlow;
      if (!(this.baseFlow.Project == "GelenEFatura"))
        return;
      this._vrFaturaID = (FlowVariable) this.baseFlow.GetObjectByName(nameof (_vrFaturaID));
      this.vrZarfID = (FlowVariable) this.baseFlow.GetObjectByName(nameof (vrZarfID));
      this.gelisZamani = (FlowVariable) this.baseFlow.GetObjectByName(nameof (gelisZamani));
      this.sonHatirlatma = (FlowVariable) this.baseFlow.GetObjectByName(nameof (sonHatirlatma));
      this.AkisDurdurucu1 = (FlowPauser) this.baseFlow.GetObjectByName(nameof (AkisDurdurucu1));
      this.hatirlatmaAktif = (FlowVariable) this.baseFlow.GetObjectByName(nameof (hatirlatmaAktif));
    }

    public void OnSessionStart()
    {
    }

    public void OnSessionEnd()
    {
    }

    public void OnSessionError(string ErrorMessage)
    {
    }

    public void fnExecuteJobs_Execute()
    {
      this.GetInvoices();
      this.StartInvoiceProcess();
      this.AttachAndStartProcesses();
    }

    public void AttachAndStartProcesses()
    {
      string empty = string.Empty;
      using (Helper helper = new Helper("990ca8f5-ca99-46ea-bd38-a7d1b503455b"))
      {
        string str1 = helper.GetCell("SELECT TOP (1) txtServerInstance FROM E_AttachAndStartProcessSettings_Form").ToString();
        foreach (DataRow row in (InternalDataCollectionBase) helper.GetDataTable("SELECT E_AttachAndStartProcessSettings_Form_DetayTablo1.txtDizin, E_AttachAndStartProcessSettings_Form_DetayTablo1.txtProjeAd, E_AttachAndStartProcessSettings_Form_DetayTablo1.txtFormAd FROM E_AttachAndStartProcessSettings_Form INNER JOIN E_AttachAndStartProcessSettings_Form_DetayTablo1 ON E_AttachAndStartProcessSettings_Form.ID = E_AttachAndStartProcessSettings_Form_DetayTablo1.FORMID").Rows)
        {
          if (Directory.Exists(row["txtDizin"].ToString()))
          {
            string path = row["txtDizin"].ToString() + "\\input";
            if (!Directory.Exists(path))
              Directory.CreateDirectory(path);
            if (Directory.GetFiles(path).Length > 0)
            {
              eBAConnection eBaConnection1 = new eBAConnection();
              eBaConnection1.Server = (str1);
              eBaConnection1.UserID = ("codeN");
              eBaConnection1.Password = ("codeNNedoc");
              using (eBAConnection eBaConnection2 = eBaConnection1)
              {
                try
                {
                  eBaConnection2.Open();
                  try
                  {
                    foreach (string file in Directory.GetFiles(path))
                    {
                      if (file.Contains("pdf") || file.Contains("txt"))
                      {
                        try
                        {
                          WorkflowDocument document = eBaConnection2.WorkflowManager.CreateDocument(row["txtProjeAd"].ToString(), row["txtFormAd"].ToString());
                          string str2 = string.Format("workflow/{0}/{1}/{2}.wfd", (object) row["txtProjeAd"].ToString(), (object) row["txtFormAd"].ToString(), (object) document.DocumentId);
                          eBaConnection2.FileSystem.GetFile(str2);
                          FileSystem fileSystem = eBaConnection2.FileSystem;
                          FileStream fileStream = new FileStream(file, FileMode.Open);
                          fileSystem.UploadFileAttachmentContentFromStream(str2, "default", Path.GetFileName(file), (Stream) fileStream);
                          fileStream.Close();
                          fileStream.Dispose();
                          WorkflowProcess process = eBaConnection2.WorkflowManager.CreateProcess(row["txtProjeAd"].ToString());
                          ((Dictionary<string, string>) process.Parameters).Add("Filename", str2);
                          process.Parameters.Update();
                          process.Start();
                          WorkflowProcessParameters parameters = process.Parameters;
                          int num = process.ProcessId;
                          string str3 = num.ToString();
                          ((Dictionary<string, string>) parameters).Add("PID", str3);
                          process.Parameters.Update();
                          if (!Directory.Exists(string.Format("{0}\\output", (object) row["txtDizin"].ToString())))
                            Directory.CreateDirectory(string.Format("{0}\\output", (object) row["txtDizin"].ToString()));
                          string fileName = Path.GetFileName(file);
                          string str4 = fileName.Substring(fileName.LastIndexOf('.') + 1);
                          string str5 = fileName.Remove(fileName.LastIndexOf('.'));
                          string sourceFileName = file;
                          string str6 = row["txtDizin"].ToString();
                          string[] strArray1 = new string[10];
                          strArray1[0] = str5;
                          strArray1[1] = "_";
                          string[] strArray2 = strArray1;
                          num = DateTime.Now.Year;
                          string str7 = num.ToString();
                          strArray2[2] = str7;
                          string[] strArray3 = strArray1;
                          num = DateTime.Now.Month;
                          string str8 = num.ToString().PadLeft(2, '0');
                          strArray3[3] = str8;
                          string[] strArray4 = strArray1;
                          DateTime now = DateTime.Now;
                          num = now.Day;
                          string str9 = num.ToString().PadLeft(2, '0');
                          strArray4[4] = str9;
                          string[] strArray5 = strArray1;
                          now = DateTime.Now;
                          num = now.Hour;
                          string str10 = num.ToString().PadLeft(2, '0');
                          strArray5[5] = str10;
                          string[] strArray6 = strArray1;
                          now = DateTime.Now;
                          num = now.Minute;
                          string str11 = num.ToString().PadLeft(2, '0');
                          strArray6[6] = str11;
                          string[] strArray7 = strArray1;
                          now = DateTime.Now;
                          num = now.Second;
                          string str12 = num.ToString().PadLeft(2, '0');
                          strArray7[7] = str12;
                          strArray1[8] = ".";
                          strArray1[9] = str4;
                          string str13 = string.Concat(strArray1);
                          string destFileName = string.Format("{0}\\output\\{1}", (object) str6, (object) str13);
                          File.Copy(sourceFileName, destFileName);
                          File.Delete(file);
                        }
                        catch (Exception ex)
                        {
                          if (!File.Exists(row["txtDizin"].ToString() + "\\Logs.txt"))
                            File.Create(row["txtDizin"].ToString() + "\\Logs.txt");
                          using (StreamWriter streamWriter = new StreamWriter(row["txtDizin"].ToString() + "\\Logs.txt", true))
                          {
                            try
                            {
                              streamWriter.WriteLine(ex.ToString());
                              streamWriter.WriteLine();
                            }
                            finally
                            {
                              streamWriter.Close();
                            }
                          }
                        }
                      }
                    }
                  }
                  finally
                  {
                    eBaConnection2.Close();
                  }
                }
                catch (Exception ex)
                {
                  using (StreamWriter streamWriter = new StreamWriter(row["txtDizin"].ToString() + "\\Logs.txt", true))
                  {
                    try
                    {
                      streamWriter.WriteLine(ex.ToString());
                      streamWriter.WriteLine();
                    }
                    finally
                    {
                      streamWriter.Close();
                    }
                  }
                }
              }
            }
          }
        }
      }
    }

    private void StartInvoiceProcess()
    {
      using (Helper CDNHelper = new Helper(this.baseFlow))
      {
        using (Integration integration = new Integration(CDNHelper))
        {
          foreach (DataRow row in (InternalDataCollectionBase) integration.GetData("codeN", "Oyak_EFatura_Bekleyenler").Rows)
          {
            using (Process process = new Process(CDNHelper))
            {
              List<eBAParameter> eBaParameterList = new List<eBAParameter>();
              eBaParameterList.Add(new eBAParameter("_vrFaturaID", row["faturaninEvrenselTekilNumarasi"].ToString()));
              eBaParameterList.Add(new eBAParameter("vrZarfID", row["zarfID"].ToString()));
              process.Parameters = (eBaParameterList);
              int processID = process.Create("GelenEFatura", "workflow", new KeyValuePair<string, string>[2]
              {
                new KeyValuePair<string, string>("_vrFaturaID", row["faturaninEvrenselTekilNumarasi"].ToString()),
                new KeyValuePair<string, string>("vrZarfID", row["zarfID"].ToString())
              });
              if (processID != 0)
              {
                this.SetCreatedProcessID(CDNHelper, row, processID);
              }
              else
              {
                string str = row["faturaninEvrenselTekilNumarasi"].ToString();
                CDNHelper.ExecuteNonQuery("UPDATE codeN_E_Invoice SET STATUS=2,DESCRIPTION='" + process.Message + "' WHERE faturaninEvrenselTekilNumarasi = '" + str + "'");
              }
            }
          }
        }
      }
    }

    private void GetInvoices()
    {
      using (Helper helper = new Helper(this.baseFlow))
      {
        using (Integration integration = new Integration(helper))
          integration.GetData("codeN_EFatura_WebService", nameof (GetInvoices));
      }
    }

    public void fnAcceptInvoice_Execute()
    {
      this.hatirlatmaAktif.Value = ("0");
      using (Helper helper = new Helper(this.baseFlow))
      {
        using (Integration integration = new Integration(helper))
          integration.GetData("codeN_EFatura_WebService", "AcceptInvoice", new SqlParameter[2]
          {
            new SqlParameter("faturaID", (object) this._vrFaturaID.Value),
            new SqlParameter("zarfID", (object) this.vrZarfID.Value)
          });
      }
    }

    public void fnRejectInvoice_Execute()
    {
      this.hatirlatmaAktif.Value = ("0");
      using (Helper helper = new Helper(this.baseFlow))
      {
        using (Integration integration = new Integration(helper))
          integration.GetData("codeN_EFatura_WebService", "RejectInvoice", new SqlParameter[2]
          {
            new SqlParameter("faturaID", (object) this._vrFaturaID.Value),
            new SqlParameter("zarfID", (object) this.vrZarfID.Value)
          });
      }
    }

    public void fnSendInvoiceToArchive_Execute()
    {
      using (Helper helper = new Helper(this.baseFlow))
      {
        using (Integration integration = new Integration(helper))
          integration.GetData("codeN_EFatura_WebService", "SendInvoiceToArchive", new SqlParameter[2]
          {
            new SqlParameter("faturaID", (object) this._vrFaturaID.Value),
            new SqlParameter("zarfID", (object) this.vrZarfID.Value)
          });
      }
    }

    public void fnConfigPauser_Execute()
    {
      DateTime dateTime1 = Convert.ToDateTime(this.gelisZamani.Value);
      dateTime1.AddDays(5.0);
      DateTime dateTime2 = !string.IsNullOrEmpty(this.sonHatirlatma.Value) ? Convert.ToDateTime(this.sonHatirlatma.Value) : new DateTime(dateTime1.AddDays(1.0).Year, dateTime1.AddDays(1.0).Month, dateTime1.AddDays(1.0).Day, 9, 0, 0);
      while (dateTime2 < DateTime.Now)
        dateTime2 = dateTime2.AddDays(1.0);
      this.sonHatirlatma.Value = (dateTime2.ToString());
      TimeSpan timeSpan = dateTime2 - DateTime.Now;
      this.baseFlow.SetFlowObjectValue("AkisDurdurucu1.day", timeSpan.Days.ToString());
      this.baseFlow.SetFlowObjectValue("AkisDurdurucu1.hour", timeSpan.Hours.ToString());
      this.baseFlow.SetFlowObjectValue("AkisDurdurucu1.minute", timeSpan.Minutes.ToString());
    }

    private void SetCreatedProcessID(Helper CDNHelper, DataRow row, int processID)
    {
      string str = row["faturaninEvrenselTekilNumarasi"].ToString();
      CDNHelper.ExecuteNonQuery("UPDATE codeN_E_Invoice SET PROCESSID = " + (object) processID + ", STATUS=1 WHERE faturaninEvrenselTekilNumarasi = '" + str + "'");
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        ;
    }

    ~Service()
    {
      this.Dispose(false);
    }
  }
}
