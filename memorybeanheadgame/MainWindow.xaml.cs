
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace memorybeanheadgame
{
    public partial class MainWindow : Window
    {
        class Card
        {
            public string DisplayText { get; set; }
            public int PairId { get; set; }
        }

        private int attempts;
        private int pairsCount;
        private int matchedPairs;
        private Button firstClButton;
        private Button secondClButton;
        private DispatcherTimer timer;
        private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear the previous Games Data

            GameGrid.Children.Clear();
            GameGrid.RowDefinitions.Clear();
            GameGrid.ColumnDefinitions.Clear();
            attempts = 0;
            matchedPairs = 0;
            firstClButton = null;
            secondClButton = null;
            timer.Stop();

            // Defaults the UI

            AttemptsTextBlock.Text = $"Attempts: {attempts}";

            // Selected Items
            ListBoxItem selectedSize = (ListBoxItem)SizeListBox.SelectedItem;
            ListBoxItem selectedTheme = (ListBoxItem)ThemeListBox.SelectedItem;
            int gridSize = int.Parse(selectedSize.Tag.ToString());
            string theme = selectedTheme.Tag.ToString();

            pairsCount = (gridSize * gridSize) / 2;

            // Grid Creating

            for (int i = 0; i < gridSize; i++)
            {
                GameGrid.RowDefinitions.Add(new RowDefinition());
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Cards 

            List<Card> useableCardList = GenerateCardList(theme, pairsCount);
            useableCardList = useableCardList.OrderBy(x => random.Next()).ToList();

            // Buttons

            int cardIndex = 0;

            for (int row = 0; row < gridSize; row++)
            {
                for (int col = 0; col < gridSize; col++)
                {
                    Button btn = new Button
                    {
                        Content = "?",
                        FontSize = 24,
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(5),
                        Tag = useableCardList[cardIndex]
                    };

                    btn.Click += CardButton_Click;

                    Grid.SetRow(btn, row);
                    Grid.SetColumn(btn, col);
                    GameGrid.Children.Add(btn);

                    cardIndex++;
                }
            }
        }

        private void CardButton_Click(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled) return;

            Button clickedButton = sender as Button;

            if (clickedButton == null || clickedButton.Content.ToString() != "?") return;

            Card cardData = (Card)clickedButton.Tag;
            clickedButton.Content = cardData.DisplayText;

            if (firstClButton == null)
            {
                firstClButton = clickedButton;
            }

            else
            {
                secondClButton = clickedButton;
                attempts++;
                AttemptsTextBlock.Text = $"Attempts: {attempts}";

                CheckForMatch();
            }
        }
        private void CheckForMatch()
        {
            Card firstCard = (Card)firstClButton.Tag;
            Card secondCard = (Card)secondClButton.Tag;

            if (firstCard.PairId == secondCard.PairId)
            {
                firstClButton.IsEnabled = false;
                secondClButton.IsEnabled = false;

                firstClButton = null;
                secondClButton = null;
                matchedPairs++;

                if (matchedPairs == pairsCount)
                {
                    MessageBox.Show($"You found all pairs in {attempts} attempts!",
                            "Congratulations!",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                }
            }
            else
            {
                timer.Start();
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            firstClButton.Content = "?";
            secondClButton.Content = "?";

            firstClButton = null;
            secondClButton = null;
        }

        private List<Card> GenerateCardList(string theme, int totalPairs)
        {
            List<Card> cardList = new List<Card>();
            List<Tuple<string, string>> availablePairs = new List<Tuple<string, string>>();

            if (theme == "Numbers")
            {
                for (int i = 1; i <= totalPairs; i++)
                {
                    availablePairs.Add(new Tuple<string, string>(i.ToString(), i.ToString()));
                }
            }
            else if (theme == "Smileys")
            {
                string[] smileys = { ":-)", ":-(", ":-D", ";-)", ":-P", "8-)", ">_<", "^_^", "-_-", "O_O", "T_T", ":-O", ":-|", "x_x", "OwO", ":-*", "B-)", "<3" };
                for (int i = 0; i < 18; i++)
                    availablePairs.Add(new Tuple<string, string>(smileys[i], smileys[i]));
            }
            else if (theme == "Countries")
            {
                availablePairs = new List<Tuple<string, string>>
                {
                    Tuple.Create("Magyarország", "Budapest"), Tuple.Create("Franciaország", "Párizs"),
                    Tuple.Create("Németország", "Berlin"), Tuple.Create("Olaszország", "Róma"),
                    Tuple.Create("Spanyolország", "Madrid"), Tuple.Create("Anglia", "London"),
                    Tuple.Create("Ausztria", "Bécs"), Tuple.Create("Japán", "Tokió"),
                    Tuple.Create("USA", "Washington"), Tuple.Create("Kína", "Peking"),
                    Tuple.Create("Görögország", "Athén"), Tuple.Create("Svédország", "Stockholm"),
                    Tuple.Create("Lengyelország", "Varsó"), Tuple.Create("Oroszország", "Moszkva"),
                    Tuple.Create("Portugália", "Lisszabon"), Tuple.Create("Svájc", "Bern"),
                    Tuple.Create("Kanada", "Ottawa"), Tuple.Create("Csehország", "Prága")
                };
                
            }

            for (int i = 0; i < totalPairs; i++)
            {
                cardList.Add(new Card { DisplayText = availablePairs[i].Item1, PairId = i });
                cardList.Add(new Card { DisplayText = availablePairs[i].Item2, PairId = i });
            }

            return cardList;
        }
    }
}