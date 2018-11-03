using System;
using System.Drawing;
using System.Windows.Forms;

namespace RomLauncher
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public sealed class NameForm : Form
    {
        /// <summary>
        /// </summary>
        private readonly string[] _names =
        {
            "Abelho", "Abrantes",
            "Abreu", "Afonso", "Águeda", "Aguiar", "Aires", "Albuquerque",
            "Alcaide", "Alcântara", "Aldeia", "Aleixo", " Alenquer",
            "Lencastre", "Almada", "Almeida", "Alvarenga", "Álvares", "Alves",
            "Alvim", "Amado", "Amaral", "Amarante", "Amaro", "Amorim", "Andrade",
            "Anjos", "Antas", "Antunes", "Aragão", "Aranha",
            "Araújo", "Areosa", "Arouca", "Arruda", "Assis", "Assunção", "Ataíde", "Aveiro",
            "Avelar", "Ávila", "Azambuja", "Azenha", "Azevedo", "Baía", "Baião",
            "Bairros", "Balsemão", "Bandeira", "Baptista", "Barata", "Barbalho", "Barbosa",
            "Barcelos", "Barra", "Barreiro", "Barrocas", "Barroqueiro", "Barros", "Barroso",
            "Bastos", "Batata", "Belchior", "Belém", "Belmonte", "Belo", "Beltrão", "Benevides",
            "Bensaúde", "Bentes", "Battencourt", "Betancour", "Bento", "Bernardes", "Bessa",
            "Bezerra", "Bicalho", "Bicudo", "Bivar", "Boaventura", "Boeira", "Farias", "Faustino",
            "Bonilha", "Borba", "Borges", "Botelho", "Botica", "Brandão", "Braga", "Bragança",
            "Branco", "Brás", "Brasil", "Brites", "Brito", "Bugalho", "Bulhões", "Bulhosa",
            "Cabral", "Cabreira", "Cachão", "Caeiro", "Caetano", "Café", "Caiado", "Caires",
            "Caldas", "Caldeira", "Camacho", "Câmara", "Cambezes", "Faria", "Faro",
            "Camelo", "Camilo", "Caminha", "Campelo", "Campos", "Canário",
            "Candeias", "Canedo", "Canela", "Canhão", "Caniça", "Cantanhede", "Capucho",
            "Cardoso", "Carlos", "Carmona", "Carneiro", "Carrasco", "Carrasqueira", "Carreira",
            "Carreiro", "Cartaxo", "Carvalhais", "Carvalhal", "Carvalheira", "Carvelheiro",
            "Carvalho", "Carvalhosa", "Carvalhoso", "Casqueira", "Castanheira",
            "Castanho", "Castelhano", "Castelo", "Castilho", "Castro", "Cascais", "Cavadas",
            "Cavadas", "Cedro", "Cerqueira", "Cerveira", "César", "Cesário", "Chagas",
            "Chamusca", "Chaves", "Cidreira", "Cipriano", "Clementino", "Coelho", "Coimbra",
            "Colaço", "Colares", "Conceição", "Conde", "Cordiero", "Correia", "Corte-Real",
            "Cortês", "Cortesão", "Costa", "Coutinho", "Couto", "Covilhã", "Covinha", "Cruz",
            "Cunha", "Custódio", "Damásio", "Dâmaso", "Dinis", "Dourado", "Domingos", "Domingues",
            "Duarte", "Durão", "Eanes", "Eiró", "Escobar", "Esparteiro", "Estrada", "Estrela",
            "Faia", "Fagundes", "Falcão", "Farinha", "Feitosa", "Felgueiras", "Félix",
            "Fernandes", "Ferrão", "Ferraz", "Ferreira", "Ferro", "Festas", "Fialho", "Fidalgo",
            "Figueiras", "Figueira", "Figueiredo", "Figueiró", "Filipe", "Fitas", "Flores",
            "Fonseca", "Fontes", "Fortunato", "Frade", "Fragoso", "França", "Franco", "Freire",
            "Freitas", "Freixo", "Frota", "Furtado", "Gago", "Galante", "Galvão", "Gama", "Gameiro",
            "Garcia", "Garrido", "Gaspar", "Gentil", "Gil", "Ginjeira", "Godinho", "Góis", "Gomes",
            "Gonçalves", "Gouveia", "Graça", "Grande", "Granja", "Gravato", "Grilo", "Guedelha",
            "Guerra", "Guerreiro", "Guimarães", "Gusmão", "Guterres", "Henriques", "Hilário",
            "Hernandes", "Hipólito", "Holanda", "Homem", "Horta", "Igrejas", "Ilha", "Imperial",
            "Inácio", "Inês", "Infante", "Jardim", "Jesus", "Jordão", "Jorge", "Judite", "Júdice",
            "Junqueira", "Lacerda", "Lages", "Lago", "Lagoa", "Lagos", "Lamego", "Lameira",
            "Lampreia", "Landim", "Laranjeira", "Laureano", "Leal", "Leão", "Leiria", "Leitão", "Leite",
            "Lemes", "lemos", "Letras", "Lima", "Linhares", "Lisboa", "Lobato", "Lobo", "Lopes",
            "Loureiro", "Lousã", "Lousada", "Lucas", "lucena", "Luz", "Macedo", "Macena", "Machado",
            "Macieira", "Maciel", "Madeira", "Madruga", "Madureira", "Mafra", "Magalhães", "Maia",
            "Maior", "Malheiro", "Malho", "Malta", "Mangueira", "Manso", "Maranhão", "Marinho",
            "Marques", "Marreiro", "Marroquim", "Martinho", "Mascarenhas", "Mata", "Matos", "Matoso",
            "Medeiros", "Medina", "Meira", "Meireles", "Melgaço", "Melo", "Mendes", "Mendonça",
            "Meneses", "Mesquita", "Mexia", "Miguel", "Milheiro", "Minho", "Miranda", "Mirandela",
            "Modesto", "Moita", "Monforte", "Monsanto", "Monte", "Montenegro", "Monteiro", "Morais",
            "Moreira", "Moreno", "Morgado", "Mortágua", "Mota", "Moura", "Mourão", "Mourato",
            "Mourinho", "Moutinho", "Moniz", "Nascimento", "Natal", "naves", "Negrão", "Negreiros",
            "Neiva", "Neto", "Neves", "Nobre", "Nóbrega", "Nogueira", "Noite", "Noronha_", "Novais",
            "Nunes", "Oleiro", "Olivares", "Oliveira", "Onofre", "Osório", "Ourique", "Outeiro",
            "Pacheco", "Padilha", "Paião", "pais", "Paiva", "Paixão", "Palha", "Palhares", "Palma",
            "Palmeira", "Pamplona", "Parafita", "Paranhos", "Pardo", "Paredes", "Parente", "Parracho",
            "Parreira", "Passarinho", "Passos", "Pastana", "Patrício", "Paula", "Paz", "Paçanha",
            "Pederneiras", "Pedroso", "Pegado", "Peixoto", "Penha", "Penteado", "Peralta", "Perdigão",
            "Pequeno", "Pessoa", "Pestana", "Picanço", "Pimenta", "Pimentel", "Pina", "Pinhal", "Pinheiro",
            "Pinto", "Pinho", "Pires", "Poças", "Pontes", "Portela", "Porto", "Portugal", "Póvoas",
            "Prada", "Prado", "Proença", "Prudente", "Quadros", "Quaresma", "Queirós", "Quental",
            "Quinaz", "Quinta", "Quintal", "Quintas", "Quintais", "Quintão", "Quintela", "Rabelo",
            "Ramalho", "Ramires", "Ramos", "Rangel", "Raposo", "Rebelo", "Rebocho", "Regalado",
            "Rêgo", "Regueira", "Reino", "Reis", "Resende", "Ribas", "Ribeiro", "Rico", "Rijo", "Rios",
            "Robalo", "Rocha", "Rodrigues", "Rosa", "Rosário", "Rosmaninho", "Rua", "Ruas", "Ruela",
            "Sá", "Sabrosa", "Sacadura", "Sacramento", "Salazar", "Saldanha", "Salgado", "Salgueiro",
            "Salvador", "Saloio", "Salomão", "Sampaio", "Sanches", "Santana", "Santarém", "Santiago",
            "Santos", "Saraiva", "Sardinha", "Seabra", "Seixas", "Semedo", "Serpa", "Sesimbra",
            "Setúbal", "Severiano", "Severo", "Silva", "Silveira", "Silvestre", "Simões", "Simão",
            "Sintra", "Soares", "Sobral", "Sobreira", "Sobrado", "Soeiro", "Sousa", "Souto", "Taborda",
            "Tavares", "Taveira", "Távora", "Teixeira", "Teles", "Teodoro", "Terra", "Tigre", "Toledo",
            "Tomé", "Torrado", "Torreiro", "Torres", "Toscano", "Travassos", "Trindade", "Tristao",
            "Valadares", "Vale", "Valente", "Valentim", "Valério", "Valverde", "Varanda", "Varela",
            "Vargas", "Vasconcelos", "Vasques", "Vaz", "Veiga", "Velasques", "Veleda", "Veloso", "Ventura",
            "Veríssimo", "Viana", "Vidal", "Vides", "Viegas", "Vieira", "Vilaça", "Vilalobos", "Vilanova",
            "Vilar", "Vilarinho", "Vilas-Boas", "Vilaverde", "Vilela", "Vilhena", "Viveiros", "Xavier",
            "Ximenes", "Zambujal", "Zarco"
        };

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public NameForm()
        {
            var buttonNext = new Button
            {
                Dock = DockStyle.Fill,
                BackColor = Color.DarkGray,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(142, 52),
                Margin = new Padding(8),
                Name = "buttonNext",
                Size = new Size(100, 28),
                TabIndex = 1,
                Text = "Next",
                UseVisualStyleBackColor = false
            };

            var labelText = new Label
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom,
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 10.25F, FontStyle.Regular, GraphicsUnit.Point, 0),
                Location = new Point(173, 0),
                Margin = new Padding(4, 0, 4, 0),
                Name = "labelText",
                Size = new Size(37, 44),
                TabIndex = 0,
                Text = Next(),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var tableLayoutPanel = new TableLayoutPanel
            {
                BackColor = Color.LightGray,
                ColumnCount = 1,
                Dock = DockStyle.Fill,
                Location = new Point(0, 0),
                Name = "tableLayoutPanel",
                RowCount = 2,
                Size = new Size(384, 88),
                TabIndex = 2
            };

            SuspendLayout();
            tableLayoutPanel.SuspendLayout();
            buttonNext.Click += (sender, arguments) => labelText.Text = Next();
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel.Controls.Add(labelText, 0, 0);
            tableLayoutPanel.Controls.Add(buttonNext, 0, 1);
            AutoScaleDimensions = new SizeF(8F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(384, 88);
            Controls.Add(tableLayoutPanel);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Margin = new Padding(4, 4, 4, 4);
            Name = "NameForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "NameGenerator";
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ResumeLayout(false);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private string Next()
        {
            return _names[new Random().Next(0, _names.Length)];
        }
    }
}