using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using GoogleApiCrawl.Models;
using GoogleApiCrawl.Converters;
using HtmlAgilityPack;

namespace GoogleApiCrawl
{
    public partial class FMain : Form
    {
        //Variavel responsavel que armazena o botão selecionado
        private Button btn_selecionado;
        public FMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Traz a posiçãominicial dos botões
        /// </summary>
        private void ConfigButtonInit()
        {
            button1.Text = "1";
            button2.Text = "2";
            button3.Text = "3";
            button4.Text = "4";
            button5.Text = "5";
            button6.Text = "6";
            button7.Text = "7";
            button8.Text = "8";
            button9.Text = "9";
            button10.Text = "10";
        }

        /// <summary>
        /// Seleciona a pagina, e muda a cor do botão se o mesmo for o botão selecionado
        /// </summary>
        private void SelectPage()
        {
            button1.BackColor = btn_selecionado.Name != button1.Name ? Color.Transparent : Color.Gray;
            button2.BackColor = btn_selecionado.Name != button2.Name ? Color.Transparent : Color.Gray;
            button3.BackColor = btn_selecionado.Name != button3.Name ? Color.Transparent : Color.Gray;
            button4.BackColor = btn_selecionado.Name != button4.Name ? Color.Transparent : Color.Gray;
            button5.BackColor = btn_selecionado.Name != button5.Name ? Color.Transparent : Color.Gray;
            button6.BackColor = btn_selecionado.Name != button6.Name ? Color.Transparent : Color.Gray;
            button7.BackColor = btn_selecionado.Name != button7.Name ? Color.Transparent : Color.Gray;
            button8.BackColor = btn_selecionado.Name != button8.Name ? Color.Transparent : Color.Gray;
            button9.BackColor = btn_selecionado.Name != button9.Name ? Color.Transparent : Color.Gray;
            button10.BackColor = btn_selecionado.Name != button10.Name ? Color.Transparent : Color.Gray;
        }

        /// <summary>
        /// Seleciona a página do botão e faz a pesquisa
        /// </summary>
        /// <param name="button"></param>
        private void SelectButton(Button button)
        {
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                string numberPage = button.Text;
                button.BackColor = Color.Gray;
                btn_selecionado = button;
                //Formula para definir o parametro de paginação do google
                numberPage = ((int.Parse(numberPage) * 10) - 10).ToString();
                GetResultsGoogle(numberPage);
                SelectPage();
            }
        }

        /// <summary>
        /// Cria os panels de visualização dos resultados
        /// </summary>
        /// <param name="resultados"></param>
        private void CreatePanelResult(Resultados resultados)
        {
            // 
            // pResult
            // 
            Panel pResults = new Panel();
            TextBox txtDescription = new TextBox();
            Label lbLink = new Label();
            Label lbTitle = new Label();
            this.flowLayoutPanel2.Controls.Add(pResults);
            pResults.Controls.Add(txtDescription);
            pResults.Controls.Add(lbLink);
            pResults.Controls.Add(lbTitle);
            pResults.Location = new System.Drawing.Point(3, 3);
            pResults.Name = "pResult";
            pResults.Size = new System.Drawing.Size(572, 115);
            pResults.TabIndex = 0;
            // 
            // lbTitle
            // 
            lbTitle.AutoSize = true;
            lbTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lbTitle.ForeColor = System.Drawing.Color.Blue;
            lbTitle.Location = new System.Drawing.Point(15, 10);
            lbTitle.Name = "lbTitle";
            lbTitle.Size = new System.Drawing.Size(45, 22);
            lbTitle.TabIndex = 0;
            lbTitle.Text = resultados.Title;
            // 
            // lbLink
            // 
            lbLink.AutoSize = true;
            lbLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lbLink.ForeColor = System.Drawing.Color.Green;
            lbLink.Location = new System.Drawing.Point(18, 34);
            lbLink.Name = "lbLink";
            lbLink.Size = new System.Drawing.Size(29, 17);
            lbLink.TabIndex = 1;
            lbLink.Text = resultados.Link;
            // 
            // txtDescription
            // 
            txtDescription.BackColor = System.Drawing.SystemColors.Control;
            txtDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            txtDescription.Location = new System.Drawing.Point(19, 54);
            txtDescription.Multiline = true;
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new System.Drawing.Size(537, 52);
            txtDescription.TabIndex = 2;
            txtDescription.ReadOnly = true;
            txtDescription.Text = (!string.IsNullOrEmpty(resultados.TimePublished) ? resultados.TimePublished + " - " : string.Empty) + resultados.Description;
        }
        /// <summary>
        /// Método responsável pela pesquisa no google
        /// </summary>
        /// <param name="numberPage"></param>
        private void GetResultsGoogle(string numberPage)
        {
            this.flowLayoutPanel2.Controls.Clear();
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                WebClient webClient = new WebClient();
                string url = "https://www.google.com/search?q=" + Regex.Replace(txtSearch.Text, " ", "+") + "&start=" + numberPage.Trim();
                string page = webClient.DownloadString(url);
                var htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(page);

                HtmlNodeCollection nodeList = htmlDocument.GetElementbyId("ires").ChildNodes;
                //Verifica se consulta gerou resultados
                if (nodeList.Count > 0)
                {
                    foreach (HtmlNode node in nodeList[0]?.ChildNodes)
                    {
                        if (node.Attributes.Count > 0)
                            if (node.Descendants().ToList().Exists(p => p.Attributes["class"] != null && p.Attributes["class"].Value.Equals("hJND5c")))
                            {
                                Resultados resultados = new Resultados();
                                resultados.Link = node.Descendants().First(p => p.Attributes["class"] != null &&
                                                                                p.Attributes["class"].Value.Equals("hJND5c")).InnerText;
                                resultados.Description = node.Descendants().First(p => p.Attributes["class"] != null &&
                                                                                  p.Attributes["class"].Value.Equals("st")).InnerText;
                                resultados.Title = node.Descendants().First(p => p.Attributes["class"] != null &&
                                                                                 p.Attributes["class"].Value.Equals("r")).InnerText.Replace("\n", "").Replace("  ", " ");
                                if (node.Descendants().ToList().Exists(p => p.Attributes["class"] != null &&
                                                                            p.Attributes["class"].Value.Equals("f")))
                                    resultados.TimePublished = node.Descendants().First(p => p.Attributes["class"] != null &&
                                                                                             p.Attributes["class"].Value.Equals("f")).InnerText;
                                //Criar Layouts dos Resultados
                                CreatePanelResult(resultados);
                            }
                    }
                    timer.Enabled = true;
                }
                else
                    timer.Enabled = false;
            }

        }
        /// <summary>
        /// Evento responsavel pelo click no botão pesquisa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_search_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                GetResultsGoogle("0");
                ConfigButtonInit();
                button1.BackColor = Color.Gray;
                btn_selecionado = button1;
                SelectPage();
            }
        }

        /// <summary>
        /// Evento Responsavel pelo click dos botões da paginação
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Click(object sender, EventArgs e)
        {
            SelectButton(((Button)sender));
        }

        /// <summary>
        /// Evento responsavel pelo voltar da paginação
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_back_Click(object sender, EventArgs e)
        {
            button1.Text = (int.Parse(button1.Text) - 1).ToString();
            button2.Text = (int.Parse(button2.Text) - 1).ToString();
            button3.Text = (int.Parse(button3.Text) - 1).ToString();
            button4.Text = (int.Parse(button4.Text) - 1).ToString();
            button5.Text = (int.Parse(button5.Text) - 1).ToString();
            button6.Text = (int.Parse(button6.Text) - 1).ToString();
            button7.Text = (int.Parse(button7.Text) - 1).ToString();
            button8.Text = (int.Parse(button8.Text) - 1).ToString();
            button9.Text = (int.Parse(button9.Text) - 1).ToString();
            button10.Text = (int.Parse(button10.Text) - 1).ToString();
            btn_back.Visible = button1.Text != "1";
            SelectButton(btn_selecionado);
        }

        /// <summary>
        /// Evento responsavel pelo proximo da paginação
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_next_Click(object sender, EventArgs e)
        {
            
            btn_back.Visible = true;
            button1.Text = (int.Parse(button1.Text) + 1).ToString();
            button2.Text = (int.Parse(button2.Text) + 1).ToString();
            button3.Text = (int.Parse(button3.Text) + 1).ToString();
            button4.Text = (int.Parse(button4.Text) + 1).ToString();
            button5.Text = (int.Parse(button5.Text) + 1).ToString();
            button6.Text = (int.Parse(button6.Text) + 1).ToString();
            button7.Text = (int.Parse(button7.Text) + 1).ToString();
            button8.Text = (int.Parse(button8.Text) + 1).ToString();
            button9.Text = (int.Parse(button9.Text) + 1).ToString();
            button10.Text = (int.Parse(button10.Text) + 1).ToString();
            SelectButton(btn_selecionado);
        }
        
        /// <summary>
        /// Evento responsavel pelo timer da paginação de 10 segundos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            btn_next_Click(this, new EventArgs());
        }
    }
}
