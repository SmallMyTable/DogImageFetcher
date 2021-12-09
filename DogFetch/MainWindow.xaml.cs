using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace DogFetch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static bool sortedAZ = false;
        public MainWindow()
        {
            InitializeComponent();
            InitialSync();
        }
        private async void InitialSync() => await FillBreedList();
        public async Task FillBreedList()
        {
            await DataFetching.GetBreedListAsync();
            DogData.BaseBreeds = DogData.MasterBreedClass.GetType().GetProperties().Select(f => f.Name).ToList();
            foreach (string breed in DogData.BaseBreeds)
            {
                BreedSearchBox.Items.Add(breed);
                List<string> currentBreed = (List<string>)DogData.GetPropValue(DogData.MasterBreedClass, breed);
                if (currentBreed.Count == 0)
                {
                    DogData.BreedList.Add(breed);
                    BreedListBox.Items.Add(breed);
                }
                else
                {
                    foreach (string subBreed in currentBreed)
                    {
                        DogData.BreedList.Add($"{subBreed} {breed}");
                        BreedListBox.Items.Add($"{subBreed} {breed}");
                    }
                }
            }
        }
        private async void FetchImage(string breed)
        {
            string breedName;
            if (breed.Any(Char.IsWhiteSpace))
            {
                string[] sepBreeds = breed.Split(' ');
                breedName = $"{sepBreeds[1]}/{sepBreeds[0]}";
            }
            else breedName = breed;
            string imgURL = await DataFetching.GetBreedImageAsync(breedName);
            BitmapImage imageSource = new BitmapImage(new Uri(imgURL));
            DogImage.Source = imageSource;
        }
        public void SortBreedList(object sender, RoutedEventArgs e)
        {
            if (sortedAZ)
            {
                var list = BreedListBox.Items.Cast<string>().OrderByDescending(item => item).ToList();
                BreedListBox.Items.Clear();
                foreach (string listItem in list)
                {
                    BreedListBox.Items.Add(listItem);
                }
                sortedAZ = false;
            }
            else
            {
                var list = BreedListBox.Items.Cast<string>().OrderBy(item => item).ToList();
                BreedListBox.Items.Clear();
                foreach (string listItem in list)
                {
                    BreedListBox.Items.Add(listItem);
                }
                sortedAZ = true;
            }
        }
        private void GetSubBreeds(string breed)
        {
            List<string> currentBreed = (List<string>)DogData.GetPropValue(DogData.MasterBreedClass, breed);
            foreach (string subBreed in currentBreed)
            {
                SubBreedBox.Items.Add(subBreed);
            }
        }
        private void BreedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SubBreedBox.Items.Clear();
            if (BreedListBox.SelectedItem != null)
            {
                string breedName = BreedListBox.SelectedItem.ToString();
                FetchImage(breedName);
            }
        }
        private void BreedSearchBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BreedSearchBox.SelectedItem != null)
            {
                SubBreedBox.Items.Clear();
                string breedName = BreedSearchBox.SelectedItem.ToString();
                GetSubBreeds(breedName);
                FetchImage(breedName);
            }
        }
        private void SubBreedBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SubBreedBox.SelectedItem != null && BreedSearchBox.SelectedItem != null)
            {
                string breedName = $"{BreedSearchBox.SelectedItem}/{SubBreedBox.SelectedItem}";
                FetchImage(breedName);
            }
        }
    }

    #region Data Handling
    class DataFetching
    {
        public static readonly string baseUrl = "https://dog.ceo/api/";
        public static readonly HttpClient client = new HttpClient();

        public static async Task<string> GetBreedImageAsync(string breedName)
        {
            RootMessage rootObj;
            string json;
            string dogPath = string.Empty;
            var requestPath = new Uri($"{baseUrl}breed/{breedName}/images/random");
            HttpResponseMessage response = await client.GetAsync(requestPath);
            if (response.IsSuccessStatusCode)
            {
                json = await response.Content.ReadAsStringAsync();
                rootObj = JsonConvert.DeserializeObject<RootMessage>(json);
                dogPath = rootObj.message;
            }
            return dogPath;
        }
        public static async Task GetBreedListAsync()
        {
            Root bClass;
            string json;
            var requestPath = new Uri($"{baseUrl}breeds/list/all");
            HttpResponseMessage response = await client.GetAsync(requestPath);
            if (response.IsSuccessStatusCode)
            {
                json = await response.Content.ReadAsStringAsync();
                bClass = JsonConvert.DeserializeObject<Root>(json);
                DogData.MasterBreedClass = bClass.message;
            }
        }
    }
    public static class DogData
    {
        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
        public static List<string> BreedList { get; set; } = new List<string>();
        public static Message MasterBreedClass { get; set; }
        public static List<string> BaseBreeds { get; set; } = new List<string>();
    }
    public class Root
    {
        public Message message { get; set; }
        public string status { get; set; }
    }
    public class RootMessage
    {
        public string message { get; set; }
        public string status { get; set; }
    }
    public class Message
    {
        public List<string> affenpinscher { get; set; }
        public List<string> african { get; set; }
        public List<string> airedale { get; set; }
        public List<string> akita { get; set; }
        public List<string> appenzeller { get; set; }
        public List<string> australian { get; set; }
        public List<string> basenji { get; set; }
        public List<string> beagle { get; set; }
        public List<string> bluetick { get; set; }
        public List<string> borzoi { get; set; }
        public List<string> bouvier { get; set; }
        public List<string> boxer { get; set; }
        public List<string> brabancon { get; set; }
        public List<string> briard { get; set; }
        public List<string> buhund { get; set; }
        public List<string> bulldog { get; set; }
        public List<string> bullterrier { get; set; }
        public List<string> cattledog { get; set; }
        public List<string> chihuahua { get; set; }
        public List<string> chow { get; set; }
        public List<string> clumber { get; set; }
        public List<string> cockapoo { get; set; }
        public List<string> collie { get; set; }
        public List<string> coonhound { get; set; }
        public List<string> corgi { get; set; }
        public List<string> cotondetulear { get; set; }
        public List<string> dachshund { get; set; }
        public List<string> dalmatian { get; set; }
        public List<string> dane { get; set; }
        public List<string> deerhound { get; set; }
        public List<string> dhole { get; set; }
        public List<string> dingo { get; set; }
        public List<string> doberman { get; set; }
        public List<string> elkhound { get; set; }
        public List<string> entlebucher { get; set; }
        public List<string> eskimo { get; set; }
        public List<string> finnish { get; set; }
        public List<string> frise { get; set; }
        public List<string> germanshepherd { get; set; }
        public List<string> greyhound { get; set; }
        public List<string> groenendael { get; set; }
        public List<string> havanese { get; set; }
        public List<string> hound { get; set; }
        public List<string> husky { get; set; }
        public List<string> keeshond { get; set; }
        public List<string> kelpie { get; set; }
        public List<string> komondor { get; set; }
        public List<string> kuvasz { get; set; }
        public List<string> labradoodle { get; set; }
        public List<string> labrador { get; set; }
        public List<string> leonberg { get; set; }
        public List<string> lhasa { get; set; }
        public List<string> malamute { get; set; }
        public List<string> malinois { get; set; }
        public List<string> maltese { get; set; }
        public List<string> mastiff { get; set; }
        public List<string> mexicanhairless { get; set; }
        public List<string> mix { get; set; }
        public List<string> mountain { get; set; }
        public List<string> newfoundland { get; set; }
        public List<string> otterhound { get; set; }
        public List<string> ovcharka { get; set; }
        public List<string> papillon { get; set; }
        public List<string> pekinese { get; set; }
        public List<string> pembroke { get; set; }
        public List<string> pinscher { get; set; }
        public List<string> pitbull { get; set; }
        public List<string> pointer { get; set; }
        public List<string> pomeranian { get; set; }
        public List<string> poodle { get; set; }
        public List<string> pug { get; set; }
        public List<string> puggle { get; set; }
        public List<string> pyrenees { get; set; }
        public List<string> redbone { get; set; }
        public List<string> retriever { get; set; }
        public List<string> ridgeback { get; set; }
        public List<string> rottweiler { get; set; }
        public List<string> saluki { get; set; }
        public List<string> samoyed { get; set; }
        public List<string> schipperke { get; set; }
        public List<string> schnauzer { get; set; }
        public List<string> setter { get; set; }
        public List<string> sheepdog { get; set; }
        public List<string> shiba { get; set; }
        public List<string> shihtzu { get; set; }
        public List<string> spaniel { get; set; }
        public List<string> springer { get; set; }
        public List<string> stbernard { get; set; }
        public List<string> terrier { get; set; }
        public List<string> tervuren { get; set; }
        public List<string> vizsla { get; set; }
        public List<string> waterdog { get; set; }
        public List<string> weimaraner { get; set; }
        public List<string> whippet { get; set; }
        public List<string> wolfhound { get; set; }
    }
    #endregion
}