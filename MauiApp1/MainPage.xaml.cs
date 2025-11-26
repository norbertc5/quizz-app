
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {

        List<TaskItem> loadedTasks = new List<TaskItem>();
        string turn = "p1";
        string questionText;
        string[] answersTexts;
        int correctAnserId;
        int currentQuestion = 0;
        int player1Score = 0;
        int player2Score = 0;


        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object? sender, EventArgs e)
        {
            await LoadMauiAsset();
            NextTurn();
            TourLabel.Text = $"Tura: {turn}";
        }

        async Task LoadMauiAsset()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("data.json");
            using var reader = new StreamReader(stream);

            var contents = reader.ReadToEnd();
            loadedTasks = JsonSerializer.Deserialize<List<TaskItem>>(contents) ?? loadedTasks; 
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var param = button.CommandParameter.ToString();

            AnswerHandle(int.Parse(param ?? "0"));
        }

        void NextTurn()
        {
            if(currentQuestion >= loadedTasks.Count)
            {
                string winner = (player1Score > player2Score) ? "gracz pierwszy" : (player2Score > player1Score) ? "gracz drugi" : "nikt, jest remis";
                DisplayAlert("Koniec", $"Wykorzystano wszystkie pytania. Wygrywa {winner}.", "Ok");
                return;
            }

            questionText = loadedTasks[currentQuestion].Question;
            answersTexts = (string[])loadedTasks[currentQuestion].Answers.Clone();
            correctAnserId = loadedTasks[currentQuestion].CorrectAnswerId;
            currentQuestion++;

            QuestionLabel.Text = questionText;
            AnswerButton0.Text = answersTexts[0];
            AnswerButton1.Text = answersTexts[1];
            AnswerButton2.Text = answersTexts[2];
            AnswerButton3.Text = answersTexts[3];
            //DisplayAlert("info", $"{ questionText}, {answersTexts[0]}, {correctAnserId}", "OK");
        }

        void AnswerHandle(int userAnswerId)
        {
            string message = "";
            if(userAnswerId == correctAnserId)
            {
                if(turn == "p1")
                {
                    message = "Poprawna odpowiedź. Podaj telefon drugiemu graczowi.";
                    player1Score++;
                }
                else
                {
                    message = "Poprawna odpowiedź. Podaj telefon pierwszemu graczowi.";
                    player2Score++;
                }
                UpdateScroe();
                DisplayAlert("Dobrze!", message, "OK");
            }
            else
            {
                if (turn == "p1")
                {
                    message = "Zła odpowiedź. Podaj telefon drugiemu graczowi.";
                }
                else
                {
                    message = "Zła odpowiedź. Podaj telefon pierwszemu graczowi.";
                }
                DisplayAlert("Źle!", message, "OK");
            }

            if(turn == "p1")
            {
                turn = "p2";
            }
            else
            {
                NextTurn();
                turn = "p1";
            }
            TourLabel.Text = $"Tura: {turn}";
        }

        void UpdateScroe()
        {
            ScoreLabel.Text = $"{player1Score}:{player2Score}";
        }
    }

    class TaskItem
    {
        public string Question { get; set; }
        public string[] Answers { get; set; }
        public int CorrectAnswerId { get; set; }
    }
}
