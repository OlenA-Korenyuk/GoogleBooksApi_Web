using Microsoft.VisualBasic;
using Telegram.Bot;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text;

using Telegram.Bot.Types;
using Telegram.Bot.Polling;



namespace GoogleBooksApi_Web
{
    class Program
    {
        static ITelegramBotClient bot = new TelegramBotClient("xxxxxxx");
        static HttpClient httpClient = new HttpClient();
        
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            //Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message && update.Message != null && !string.IsNullOrEmpty(update.Message.Text))
            {

                var message = update.Message;
                User user = message.From;
                string user_firstname = user.FirstName;
                long user_id = user.Id;
                var document = new BsonDocument
                {
                            { "user_id", user_id },
                            { "user_firstname", user_firstname },
                            { "bot_waiting_for_user_book", false },
                            { "bot_waiting_for_user_genre", false },
                            { "bot_waiting_for_user_unwanted_genre", false },
                            { "bot_waiting_for_user_book_you_have_read", false },
                            { "bot_waiting_for_user_favorite_author", false },
                            { "bot_waiting_for_user_book_to_delete", false },
                            { "bot_waiting_for_user_genre_to_delete", false },
                            { "bot_waiting_for_user_author_to_delete", false },
                            { "bot_waiting_for_user_secret_book", false },
                            { "genres", new BsonArray() },
                            { "already_read_books", new BsonArray() }


                };

                var filter = Builders<BsonDocument>.Filter.Eq("user_id", user_id);
                var exists = Constants.collection.Find(filter).Any();

                if (!exists)
                {
                    Constants.collection.InsertOne(document);
                }
                var bot_waiting_for_book_response = await httpClient.GetAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_book/{user_id}");
                var bot_waiting_for_book_result = await bot_waiting_for_book_response.Content.ReadAsStringAsync();
                bool bot_waiting_for_user_book = Convert.ToBoolean(bot_waiting_for_book_result);

                var bot_waiting_for_genre_response = await httpClient.GetAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_genre/{user_id}");
                var bot_waiting_for_genre_result = await bot_waiting_for_genre_response.Content.ReadAsStringAsync();
                bool bot_waiting_for_user_genre = Convert.ToBoolean(bot_waiting_for_genre_result);

                var bot_waiting_for_unwanted_genre_response = await httpClient.GetAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_unwanted_genre/{user_id}");
                var bot_waiting_for_unwanted_genre_result = await bot_waiting_for_unwanted_genre_response.Content.ReadAsStringAsync();
                bool bot_waiting_for_user_unwanted_genre = Convert.ToBoolean(bot_waiting_for_unwanted_genre_result);

                var bot_waiting_for_book_you_have_read_response = await httpClient.GetAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_book_you_have_read/{user_id}");
                var bot_waiting_for_book_you_have_read_result = await bot_waiting_for_book_you_have_read_response.Content.ReadAsStringAsync();
                bool bot_waiting_for_user_book_you_have_read = Convert.ToBoolean(bot_waiting_for_book_you_have_read_result);

                var bot_waiting_for_favorite_author_response = await httpClient.GetAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_favorite_author/{user_id}");
                var bot_waiting_for_favorite_author_result = await bot_waiting_for_favorite_author_response.Content.ReadAsStringAsync();
                bool bot_waiting_for_user_favorite_author = Convert.ToBoolean(bot_waiting_for_favorite_author_result);

                var bot_waiting_for_book_to_delete_response = await httpClient.GetAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_book_to_delete/{user_id}");
                var bot_waiting_for_book_to_delete_result = await bot_waiting_for_book_to_delete_response.Content.ReadAsStringAsync();
                bool bot_waiting_for_user_book_to_delete = Convert.ToBoolean(bot_waiting_for_book_to_delete_result);

                var bot_waiting_for_genre_to_delete_response = await httpClient.GetAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_genre_to_delete/{user_id}");
                var bot_waiting_for_genre_to_delete_result = await bot_waiting_for_genre_to_delete_response.Content.ReadAsStringAsync();
                bool bot_waiting_for_user_genre_to_delete = Convert.ToBoolean(bot_waiting_for_genre_to_delete_result);

                var bot_waiting_for_author_to_delete_response = await httpClient.GetAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_author_to_delete/{user_id}");
                var bot_waiting_for_author_to_delete_result = await bot_waiting_for_author_to_delete_response.Content.ReadAsStringAsync();
                bool bot_waiting_for_user_author_to_delete = Convert.ToBoolean(bot_waiting_for_author_to_delete_result);

                var bot_waiting_for_secret_book_response = await httpClient.GetAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_secret_book/{user_id}");
                var bot_waiting_for_secret_book_result = await bot_waiting_for_secret_book_response.Content.ReadAsStringAsync();
                bool bot_waiting_for_user_secret_book = Convert.ToBoolean(bot_waiting_for_secret_book_result);

                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Hello in the book bot! \U0001F44B Choose a command you want to execute.:\n/get_book_info - Find information about a book by its title \U0001F50D\n/get_book_by_genre - Get book recommendations based on the specified genre \U0001F4DA\n/add_unwanted_genre - Add unwanted genre \U0000274C\n/add_already_read_book - Add a book to the list of books you have read \U0001F4CC\n/add_favorite_author - Add a favorite author to the list \U0001F465\n/get_my_unwanted_genres - Get the list of genres you don't wanna read \U0001F44E\U0001F4DD\n/get_the_books_i_have_read - Get a list of books I've read \U00002705\n/get_my_favorite_authors - Get a list of my favorite authors \U0001F6D0\n/delete_book_i_have_read - Delete a book from the list of books you've read \U0001F5D1\n/delete_unwanted_genre - Delete the genre from the list of unwanted genres \U0001F5D1\n/delete_favorite_authors - Delete the author from the list of favorite authors 🗑\n/secret_book - Get a secret book by description \U0001F92B");
                    await httpClient.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_book/{user_id}?b=false", null);
                    return;
                }
                if (message.Text.ToLower() == "/get_book_info")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Please enter the book title \U0001F4DA");
                    await httpClient.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_book/{user_id}?b=true", null);
                    return;
                }

                if (bot_waiting_for_user_book)
                {
                    HttpClient search_book_client = new HttpClient();
                    try
                    {
                        var response = await search_book_client.GetAsync($"https://{Constants.host}/GoogleBooksApi?bookName={message.Text}");
                        var responseContent = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            Book book = JsonConvert.DeserializeObject<Book>(responseContent);
                            string authors = string.Join(", ", book.Authors);

                            await botClient.SendTextMessageAsync(message.Chat, $"\U000025AATitle: {book.Title}\n\U000025AAAuthors: {authors}\n▪PageCount: {book.PageCount}\n▪Publisher: {book.Publisher}");
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "There is no such book \U0001F625");
                        }

                        await search_book_client.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_book/{user_id}?b=false", null);
                        return;
                    }
                    catch (Exception ex)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "An error occurred while retrieving book information \U0001F631");
                        await search_book_client.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_book/{user_id}?b=false", null);
                        return;
                    }
                }
                if (message.Text.ToLower() == "/get_book_by_genre")
                {
                    string genreList = "Enter the number of the genre from the following list \U0001F447:\n" +
                        "1. Adventure\n" +
                        "2. Detective\n" +
                        "3. Science fiction\n" +
                        "4. Historical fiction\n" +
                        "5. Dystopian\n" +
                        "6. Romance novel\n" +
                        "7. Horror\n" +
                        "8. Classic\n" +
                        "9. Fairy tale\n" +
                        "10. Fan fiction";

                    await botClient.SendTextMessageAsync(message.Chat, genreList);
                    await httpClient.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_genre/{user_id}?b=true", null);
                    return;
                }

                if (bot_waiting_for_user_genre)
                {
                    HttpClient search_book_client = new HttpClient();
                    try
                    {
                        bool isValidGenre = int.TryParse(message.Text, out int genreNumber);
                        if (!isValidGenre || genreNumber < 1 || genreNumber > 10)
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "Invalid genre number. Please enter a valid number from the list \U0001F575");
                            await search_book_client.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_genre/{user_id}?b=false", null);
                            return;
                        }

                        string genre = GetGenreByNumber(genreNumber);

                        var response = await search_book_client.GetAsync($"https://{Constants.host}/GoogleBooksApi/genre?genre={genre}");
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var books = JsonConvert.DeserializeObject<List<Book>>(responseContent);

                        if (books.Count == 0)
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "No books were found with such genre \U0001F615");
                            await search_book_client.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_genre/{user_id}?b=false", null);
                            return;
                        }

                        var randomBooks = books.OrderBy(x => Guid.NewGuid()).Take(3).ToList();

                        foreach (var book in randomBooks)
                        {
                            string authors = string.Join("\n", book.Authors);

                            await botClient.SendTextMessageAsync(message.Chat, $"▪ Title: {book.Title}\n▪ Authors: {authors}\n▪ PageCount: {book.PageCount}\n▪ Publisher: {book.Publisher}");
                        }

                        await search_book_client.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_genre/{user_id}?b=false", null);
                        return;
                    }
                    catch (Exception ex)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Error searching for books by genre \U0001F613");
                        await search_book_client.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_genre/{user_id}?b=false", null);
                        return;
                    }

                }


                if (message.Text.ToLower() == "/add_unwanted_genre")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Enter the numbers of the unwanted genres from the following options, separated by commas:\n1. Adventure\n2. Detective\n3. Science fiction\n4. Historical fiction\n5. Dystopian\n6. Romance novel\n7. Horror\n8. Classic\n9. Fairy tale\n10. Fan fiction");

                    // Оновлюємо поле bot_waiting_for_unwanted_genre користувача в базі даних, щоб позначити, що ми очікуємо відповідь з номерами жанрів
                    await httpClient.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_unwanted_genre/{user_id}?b=true", null);
                    return;
                }

                // Якщо ми очікуємо відповідь з номерами жанрів від користувача
                if (bot_waiting_for_user_unwanted_genre)
                {
                    var genreNumbersText = message.Text.Split(',')
                                                        .Select(num => num.Trim())
                                                        .Where(num => !string.IsNullOrEmpty(num))
                                                        .ToList();

                    var genresToAdd = new List<string>();

                    foreach (var genreNumberText in genreNumbersText)
                    {
                        if (int.TryParse(genreNumberText, out int genreNumber))
                        {
                            var genre = GetGenreByNumber(genreNumber);
                            if (!string.IsNullOrEmpty(genre))
                            {
                                genresToAdd.Add(genre);
                            }
                        }
                    }

                    if (genresToAdd.Any())
                    {
                        try
                        {
                            HttpClient search_book_client = new HttpClient();

                            var addedGenres = new List<string>();

                            foreach (var genreToAdd in genresToAdd)
                            {
                                // Створюємо посилання на контролер та передаємо в нього id користувача та назву жанру
                                var url = $"https://{Constants.host}/GoogleBooksApi/user_genres/{user_id}/{genreToAdd}";
                                var content = new StringContent(JsonConvert.SerializeObject(genreToAdd), Encoding.UTF8, "application/json");

                                // Викликаємо контролер методом PUT, щоб додати новий жанр до списку небажаних жанрів
                                var response = await search_book_client.PutAsync(url, content);

                                // Перевіряємо, що відповідь успішна
                                if (response.IsSuccessStatusCode)
                                {
                                    addedGenres.Add(genreToAdd);
                                }
                            }

                            if (addedGenres.Any())
                            {
                                var addedGenresText = string.Join(", ", addedGenres);
                                await botClient.SendTextMessageAsync(message.Chat, $"The following genres have been added to your list of unwanted genres: {addedGenresText} \U0001F44C");
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(message.Chat, "None of the specified genres were added to the list \U0001F914");
                            }

                            // Оновлюємо поле bot_waiting_for_unwanted_genre користувача в базі даних
                            await httpClient.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_unwanted_genre/{user_id}?b=false", null);
                        }
                        catch (ArgumentException ex)
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "Invalid genre number(s) \U0001F613");
                        }
                        catch (Exception ex)
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "Error adding unwanted genres \U0001F480");
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Enter valid genre number(s) \U0001F620");
                    }

                    return;
                }
                if (message.Text.ToLower() == "/add_already_read_book")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Enter the title of the book you have read \U00002705:");
                    await httpClient.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_book_you_have_read/{user_id}?b=true", null);
                    return;
                }
                if (bot_waiting_for_user_book_you_have_read)
                {
                    try
                    {
                        HttpClient search_book_client = new HttpClient();
                        var url = $"https://{Constants.host}/GoogleBooksApi/user_already_read/{user_id}/{message.Text}";
                        var content = new StringContent(JsonConvert.SerializeObject(message.Text), Encoding.UTF8, "application/json");
                        var response = await search_book_client.PutAsync(url, content);

                        if (response.IsSuccessStatusCode)
                        {
                            await httpClient.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_book_you_have_read/{user_id}?b=false", null);
                            await botClient.SendTextMessageAsync(message.Chat, $"The book \"{message.Text}\" has been added to the list of books you have read \U0001F60E");
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "Error adding book \U0001F614");
                        }
                    }
                    catch (Exception ex)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Error adding book \U0001F614");
                        await botClient.SendTextMessageAsync(message.Chat, ex.Message);
                    }

                    return;
                }
                if (message.Text.ToLower() == "/add_favorite_author")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Enter your favorite author \U0001F929:");
                    await httpClient.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_favorite_author/{user_id}?b=true", null);
                    return;
                }
                if (bot_waiting_for_user_favorite_author)
                {
                    try
                    {
                        HttpClient search_book_client = new HttpClient();
                        var url = $"https://{Constants.host}/GoogleBooksApi/user_favorite_author/{user_id}/{message.Text}";
                        var content = new StringContent(JsonConvert.SerializeObject(message.Text), Encoding.UTF8, "application/json");
                        var response = await search_book_client.PutAsync(url, content);

                        if (response.IsSuccessStatusCode)
                        {
                            await httpClient.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_favorite_author/{user_id}?b=false", null);
                            await botClient.SendTextMessageAsync(message.Chat, $"The author \"{message.Text}\" has been added to the list of favorite authors \U0001F44C");
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "Error adding author \U0001F636");
                        }
                    }
                    catch (Exception ex)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Error adding book 😶");
                        await botClient.SendTextMessageAsync(message.Chat, ex.Message);
                    }

                    return;
                }

                if (message.Text.ToLower() == "/get_the_books_i_have_read")
                {
                    try
                    {
                        HttpClient get_books_client = new HttpClient();
                        var url = $"https://{Constants.host}/GoogleBooksApi/user_read_books/{user_id}";
                        var response = await get_books_client.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            var bookList = JsonConvert.DeserializeObject<List<string>>(responseContent);

                            if (bookList.Count == 0)
                            {
                                await botClient.SendTextMessageAsync(message.Chat, "You have not added any books to your reading list \U0001F92F");
                            }
                            else
                            {
                                var messageText = "List of books you have read \U0001F4D6:\n";
                                for (int i = 0; i < bookList.Count; i++)
                                {
                                    messageText += $"{i + 1}. {bookList[i]}\n";
                                }

                                await botClient.SendTextMessageAsync(message.Chat, messageText);
                            }
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "Error retrieving the list of books \U0001F928");
                        }
                    }
                    catch (Exception ex)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Error retrieving the list of books \U0001F928");
                        await botClient.SendTextMessageAsync(message.Chat, ex.Message);
                    }

                    return;
                }

                if (message.Text.ToLower() == "/get_my_favorite_authors")
                {
                    try
                    {
                        HttpClient get_books_client = new HttpClient();
                        var url = $"https://{Constants.host}/GoogleBooksApi/user_favorite_authors/{user_id}";
                        var response = await get_books_client.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            var authorList = JsonConvert.DeserializeObject<List<string>>(responseContent);

                            if (authorList.Count == 0)
                            {
                                await botClient.SendTextMessageAsync(message.Chat, "You have not added any authors to your list \U0001F92D");
                            }
                            else
                            {
                                var messageText = "List of your favorite authors \U0001F447:\n";
                                for (int i = 0; i < authorList.Count; i++)
                                {
                                    messageText += $"{i + 1}. {authorList[i]}\n";
                                }

                                await botClient.SendTextMessageAsync(message.Chat, messageText);
                            }
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "Error retrieving the list of authors \U0001F921");
                        }
                    }
                    catch (Exception ex)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Error retrieving the list of authors \U0001f921");
                       
                    }

                    return;
                }
                if (message.Text.ToLower() == "/get_my_unwanted_genres")
                {
                    try
                    {
                        HttpClient get_books_client = new HttpClient();
                        var url = $"https://{Constants.host}/GoogleBooksApi/user_unwanted_genres/{user_id}";
                        var response = await get_books_client.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            var genreList = JsonConvert.DeserializeObject<List<string>>(responseContent);

                            if (genreList.Count == 0)
                            {
                                await botClient.SendTextMessageAsync(message.Chat, "You have not added any genres to your list \U0001F921");
                            }
                            else
                            {
                                var messageText = "List of your unwanted genres \U0001F447:\n";
                                for (int i = 0; i < genreList.Count; i++)
                                {
                                    messageText += $"{i + 1}. {genreList[i]}\n";
                                }

                                await botClient.SendTextMessageAsync(message.Chat, messageText);
                            }
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "Error retrieving the list of genres \U0001F44E");
                        }
                    }
                    catch (Exception ex)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Error retrieving the list of genres 👎");
                        await botClient.SendTextMessageAsync(message.Chat, ex.Message);
                    }

                    return;
                }

                if (message.Text.ToLower() == "/delete_book_i_have_read")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Enter the title of the book you want to delete from your read books \U0001F62C:");
                    
                    // Оновлюємо поле bot_waiting_for_book_to_delete користувача в базі даних, щоб позначити, що очікується відповідь з назвою книги
                    await httpClient.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_book_to_delete/{user_id}?b=true", null);
                    return;
                }

                // Якщо ми очікуємо відповідь з назвою книги від користувача
                if (bot_waiting_for_user_book_to_delete)
                {
                    try
                    {
                        HttpClient delete_book_client = new HttpClient();

                        // Створюємо посилання на контролер та передаємо в нього id користувача та назву книги
                        var url = $"https://{Constants.host}/GoogleBooksApi/delete_book_ive_read/{user_id}/{message.Text}";

                        // Викликаємо контролер методом DELETE, щоб видалити книгу зі списку прочитаних книг
                        var response = await delete_book_client.DeleteAsync(url);

                        // Перевіряємо, що відповідь успішна
                        if (response.IsSuccessStatusCode)
                        {
                            // Оновлюємо поле bot_waiting_for_book_to_delete користувача в базі даних
                            await httpClient.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_book_to_delete/{user_id}?b=false", null);

                            // Повідомляємо користувача, що книга була успішно видалена зі списку прочитаних книг
                            await botClient.SendTextMessageAsync(message.Chat, $"The book \"{message.Text}\" has been deleted from your read books \U0001F918");
                        }
                        else
                        {
                            await httpClient.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_book_to_delete/{user_id}?b=false", null);
                            await botClient.SendTextMessageAsync(message.Chat, "There is no such book in the list or you entered the wrong title \U0001F440");
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Error deleting the book 👀");
                        await botClient.SendTextMessageAsync(message.Chat, ex.Message);
                    }

                    return;
                }

                if (message.Text.ToLower() == "/delete_unwanted_genre")
                {
                    var unwantedGenres = new List<string>
    {
        "Adventure", "Detective", "Science fiction", "Historical fiction", "Dystopian",
        "Romance novel", "Horror", "Classic", "Fairy tale", "Fan fiction"
    };

                    var genreOptions = unwantedGenres.Select((genre, index) => $"{index + 1}. {genre}").ToList();
                    var genreOptionsText = string.Join("\n", genreOptions);

                    await botClient.SendTextMessageAsync(message.Chat, $"Enter the numbers of the genres you want to delete, separated by commas:\n{genreOptionsText}");

                    // Оновлюємо поле bot_waiting_for_genre_to_delete користувача в базі даних, щоб позначити, що очікується відповідь з номерами жанрів
                    await httpClient.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_genre_to_delete/{user_id}?b=true", null);
                    return;
                }

                // Якщо ми очікуємо відповідь з номерами жанрів від користувача
                if (bot_waiting_for_user_genre_to_delete)
                {
                    var unwantedGenres = new List<string>
    {
        "Adventure", "Detective", "Science fiction", "Historical fiction", "Dystopian",
        "Romance novel", "Horror", "Classic", "Fairy tale", "Fan fiction"
    };

                    var genreNumbersText = message.Text.Split(',')
                                                        .Select(num => num.Trim())
                                                        .Where(num => !string.IsNullOrEmpty(num))
                                                        .ToList();

                    var genresToDelete = new List<string>();

                    foreach (var genreNumberText in genreNumbersText)
                    {
                        if (int.TryParse(genreNumberText, out int genreNumber) && genreNumber >= 1 && genreNumber <= unwantedGenres.Count)
                        {
                            genresToDelete.Add(unwantedGenres[genreNumber - 1]);
                        }
                    }

                    if (genresToDelete.Any())
                    {
                        try
                        {
                            HttpClient delete_genre_client = new HttpClient();
                            var addedGenres = new List<string>();

                            foreach (var genreToDelete in genresToDelete)
                            {
                                // Створюємо посилання на контролер та передаємо в нього id користувача та назву жанру
                                var url = $"https://{Constants.host}/GoogleBooksApi/delete_unwanted_genre/{user_id}/{genreToDelete}";

                                // Викликаємо контролер методом DELETE, щоб видалити жанр зі списку небажаних жанрів
                                var response = await delete_genre_client.DeleteAsync(url);

                                // Перевіряємо, що відповідь успішна
                                if (response.IsSuccessStatusCode)
                                {
                                    addedGenres.Add(genreToDelete);
                                }
                                else
                                {
                                    await botClient.SendTextMessageAsync(message.Chat, $"Error deleting the genre \"{genreToDelete}\" 🤔");
                                    await httpClient.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_genre_to_delete/{user_id}?b=false", null);
                                }
                            }

                            if (addedGenres.Any())
                            {
                                var addedGenresText = string.Join(", ", addedGenres);
                                await botClient.SendTextMessageAsync(message.Chat, $"The following genres have been deleted from your unwanted genres list: {addedGenresText} \U0001F44C");
                                await httpClient.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_genre_to_delete/{user_id}?b=false", null);
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(message.Chat, "No genres were deleted \U0001F615");
                            }

                            // Оновлюємо поле bot_waiting_for_genre_to_delete користувача в базі даних
                            await httpClient.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_genre_to_delete/{user_id}?b=false", null);
                        }
                        catch (Exception ex)
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "Error deleting the genre 🤔");
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Please enter valid numbers corresponding to the genres you want to delete \U0001F620");
                    }

                    return;
                }

                if (message.Text.ToLower() == "/delete_favorite_authors")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Enter the author you want to delete \U0001F464:");

                    // Оновлюємо поле bot_waiting_for_genre_to_delete користувача в базі даних, щоб позначити, що очікується відповідь з назвою жанру
                    await httpClient.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_author_to_delete/{user_id}?b=true", null);
                    return;
                }

                // Якщо ми очікуємо відповідь з назвою жанру від користувача
                if (bot_waiting_for_user_author_to_delete)
                {
                    try
                    {
                        HttpClient delete_genre_client = new HttpClient();

                        // Створюємо посилання на контролер та передаємо в нього id користувача та назву жанру
                        var url = $"https://{Constants.host}/GoogleBooksApi/delete_favorite_author/{user_id}/{message.Text}";

                        // Викликаємо контролер методом DELETE, щоб видалити жанр зі списку небажаних жанрів
                        var response = await delete_genre_client.DeleteAsync(url);

                        // Перевіряємо, що відповідь успішна
                        if (response.IsSuccessStatusCode)
                        {
                            // Оновлюємо поле bot_waiting_for_genre_to_delete користувача в базі даних
                            await httpClient.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_author_to_delete/{user_id}?b=false", null);

                            // Повідомляємо користувача, що жанр був успішно видалений зі списку небажаних жанрів
                            await botClient.SendTextMessageAsync(message.Chat, $"The authors \"{message.Text}\" has been deleted from your favorite authors list \U0001F44C");
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "This author does not exist in the list or you entered the wrong name \U0001F629");
                            await httpClient.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_author_to_delete/{user_id}?b=false", null);
                        }
                    }
                    catch (Exception ex)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Error deleting the author 😩");
                        
                    }
                    await httpClient.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_author_to_delete/{user_id}?b=false", null);

                    return;
                }

                if (message.Text.ToLower() == "/secret_book")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Please enter the book description \U0001F92B");
                    await httpClient.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_secret_book/{user_id}?b=true", null);
                    return;
                }
                if (bot_waiting_for_user_secret_book)
                {
                    HttpClient search_book_client = new HttpClient();
                    try
                    {
                        var response = await search_book_client.GetAsync($"https://{Constants.host}/GoogleBooksApi/search_book/{user_id}/{Uri.EscapeDataString(message.Text)}");
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var book = JsonConvert.DeserializeObject<Book>(responseContent);

                        await botClient.SendTextMessageAsync(message.Chat, $"\U000025ABTitle : {book.Title}\n▫Authors: {string.Join(", ", book.Authors)}\n▫PageCount: {book.PageCount}\n▫Publisher: {book.Publisher}\n▫Description: {book.Description}");

                        await search_book_client.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_secret_book/{user_id}?b=false", null);
                        return;
                    }
                    catch (Exception ex)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "There is no book matching the description \U0001F937");
                        await search_book_client.PutAsync($"https://{Constants.host}/GoogleBooksApi/bot_waiting_for_secret_book/{user_id}?b=false", null);
                        return;
                    }

                }
                if (message.Type == Telegram.Bot.Types.Enums.MessageType.Sticker)
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Sorry, but stickers are not supported in this bot.");
                    return;
                }

                await botClient.SendTextMessageAsync(message.Chat, "Error. Use the bot functionality \U0001F621");


            }
        }
        private static string GetGenreByNumber(int genreNumber)
        {
            switch (genreNumber)
            {
                case 1:
                    return "Adventure";
                case 2:
                    return "Detective";
                case 3:
                    return "Science fiction";
                case 4:
                    return "Historical fiction";
                case 5:
                    return "Dystopian";
                case 6:
                    return "Romance novel";
                case 7:
                    return "Horror";
                case 8:
                    return "Classic";
                case 9:
                    return "Fairy tale";
                case 10:
                    return "Fan fiction";
                default:
                    throw new ArgumentException("Invalid genre number.");
            }
        }
        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }

        static void Main(string[] args)
        {

            

            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);
            Constants.mongoClient = new MongoClient("xxxxxx");
            Constants.database = Constants.mongoClient.GetDatabase("TelegramBot");
            Constants.collection = Constants.database.GetCollection<BsonDocument>("xxxxx");

            

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { },
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
            //Console.ReadLine();
        }

        

    }
}
