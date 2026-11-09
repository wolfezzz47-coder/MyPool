using System.Diagnostics;
using System.Text;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine(">>> Hello, myPool World!");
// working with myPool
// зробимо масив потоків, які будуть виконувати завантаження
// фото із інтернету кожен окремо у своєму потоці
// 1. Створюємо потік
//Thread myThread = new Thread(ThreadDownload);
//myThread.Start(20); //запуск потоку
////дочекатися поки потік завешить свою фото, щоб можна було працювати із даними,
////які були зроблені у потоці
//Console.WriteLine("Ми проводимо закачку даних у потоці ...");
//myThread.Join(); //очікує завеншення опреції
////дочекатися поки потік завешить свою фото, щоб можна було працювати із даними,
////які були зроблені у потоці
//Console.WriteLine("Ми проводимо закачку даних у потоці ...");

// 2. Створюємо масив потоків
Console.WriteLine(">>> Скільки потоків запустити? -> ");
int count = int.Parse(Console.ReadLine());

ConsoleColor[] palette = // палітра кольорів для виводу у консоль
    // якщо у нас буде 10 потоків, то кольори будуть повторюватися
{
    ConsoleColor.Green,
    ConsoleColor.Red,
    ConsoleColor.Cyan,
    ConsoleColor.Yellow,
    ConsoleColor.Magenta,
    ConsoleColor.Blue,
    ConsoleColor.White,
    ConsoleColor.DarkYellow
};
object consoleLock = new object();
// створюємо обєкт для блокування доступу до консолі,
// щоб потоки не заважали один одному

Thread[] threads = new Thread[count]; // створюємо масив потоків
for (int i = 0; i < count; i++)
{
    int threadNumber = i + 1; // номер потоку, який ми передаємо у лямбда-обгортку          
    ConsoleColor color = palette[i % palette.Length];
    threads[i] = new Thread(() => ThreadDownload(5, threadNumber, color));
    // щоб передати параметри у потік
    threads[i].Start();
}
// зробимо очікування завершення усіх потоків
foreach (Thread item in threads)
{
    item.Join(); //очікує завеншення опреції
}
//threads[0] = new Thread(ThreadDownload);
//threads[0].Start(5); //запуск потоку 1
//threads[1] = new Thread(ThreadDownload);
//threads[1].Start(5); //запуск потоку 2
//threads[0].Join(); //очікує завеншення опреції
//threads[1].Join(); //очікує завеншення опреції

Console.WriteLine("Операція завершена. Гарного дня");

void WriteColored(string message, ConsoleColor color)
// метод для виводу у консоль із кольором
{
    lock (consoleLock) // блокування доступу до консолі,
                       // щоб потоки не заважали один одному
    {
        Console.ForegroundColor = color;
        // обов'язково після виводу у консоль потрібно скинути колір
        Console.WriteLine(message);
        // скидаємо колір консолі
        Console.ResetColor();
    }
}

void ThreadDownload(int itemsDownload, int threadNumber, ConsoleColor color)
{
    WriteColored($"Thread {threadNumber}: Count = {itemsDownload}", color);
    // запускаємо таймер для вимірювання часу виконання
    Stopwatch stopWatch = new Stopwatch();
    stopWatch.Start();

    DownloadImages(itemsDownload, threadNumber, color);

    stopWatch.Stop();
    // Get the elapsed time as a TimeSpan value.
    TimeSpan ts = stopWatch.Elapsed;

    // Format and display the TimeSpan value.
    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
        ts.Hours, ts.Minutes, ts.Seconds,
        ts.Milliseconds / 10);
    WriteColored($"Thread {threadNumber}: RunTime " + elapsedTime, color);
    // робимо вивід у консоль із кольором
    Console.WriteLine("RunTime " + elapsedTime);
}

void DownloadImages(int count, int threadNumber, ConsoleColor color) 
    //Сиконується у окремому потоці
{
    //Масив фото, які нам потрібно скачати
    HttpClient client = new HttpClient();

    List<string> urls = new List<string>()
    {
    //    "https://content2.rozetka.com.ua/goods/images/big/671871936.jpg",
    //    "https://content.rozetka.com.ua/goods/images/big/590680165.jpg",
    //    "https://content1.rozetka.com.ua/goods/images/big/623059494.jpg",
    //    "https://content2.rozetka.com.ua/goods/images/big/569996764.jpg",
    //    "https://content1.rozetka.com.ua/goods/images/big/655330317.jpg",
    //    "https://content1.rozetka.com.ua/goods/images/big/655339086.jpg"
    };

    //string urls - можна звяти багато фото на одному цікавому сайті
    for (int i = 0; i < count; i++)
    {
        urls.Add("https://picsum.photos/800/600");
    }

    string folder = "avatars";
    //Directory
    Directory.CreateDirectory(folder); //створюємо папку для фото у поточні папці
    //Працюю з двійковими файлами - по суті це байти
    foreach (string url in urls)
    {
        //Читаю файли із інтеренати по url- байти
        byte[] buffer = client.GetByteArrayAsync(url).Result;
        //Роблю унікальне і'мя файлу
        string fileName = Guid.NewGuid().ToString() + ".jpg";
        //Обєдную ім'я папки із іменем файлу
        string path = Path.Combine(folder, fileName);
        //Зберегаю наше фото
        File.WriteAllBytes(path, buffer);
    }
}