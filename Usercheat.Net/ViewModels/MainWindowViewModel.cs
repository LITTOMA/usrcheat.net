using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Usercheat.Net.Models;

namespace Usercheat.Net.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<GameModel> games;

        public ObservableCollection<Models.GameModel> Games
        {
            get => games;
            set => this.RaiseAndSetIfChanged(ref games, value);
        }

        public async void OpenCheatFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filters = new List<FileDialogFilter>() {
                new FileDialogFilter() {
                    Name = "dat files",
                    Extensions = new List<string> { "dat" }
                }
            };
            var paths = await dialog.ShowAsync(((App)App.Current).MainWindow);

            if (paths == null)
                return;

            var cheat = new R4Cheat.R4Cheat(paths[0]);
            cheat.LoadAllGames(null);
            Games = new ObservableCollection<Models.GameModel>(
                cheat.Games.Select(game =>
                {
                    return new Models.GameModel()
                    {
                        Name = game.Name,
                        Items = new ObservableCollection<Models.CheatItemModel>(game.Items.Select<R4Cheat.R4Item, Models.CheatItemModel>(item =>
                        {
                            if (item is R4Cheat.R4Code)
                            {
                                var code = item as R4Cheat.R4Code;
                                return new Models.CheatCodeModel()
                                {
                                    Name = item.Name,
                                    Description = item.Description,
                                    Values = new ObservableCollection<int>(code.Values),
                                    Enabled = code.Enabled,
                                };
                            }
                            else if (item is R4Cheat.R4Folder)
                            {
                                var folder = item as R4Cheat.R4Folder;
                                return new Models.CheatFolderModel()
                                {
                                    Name = item.Name,
                                    Description = item.Description,
                                    Items = new ObservableCollection<Models.CheatCodeModel>(
                                        folder.Codes.Select(code =>
                                        {
                                            return new Models.CheatCodeModel()
                                            {
                                                Name = code.Name,
                                                Description = code.Description,
                                                Values = new ObservableCollection<int>(code.Values),
                                                Enabled = code.Enabled,
                                            };
                                        })),
                                    Enabled = folder.Enabled,
                                };
                            }

                            return null;
                        }))
                    };
                }));
            
        }
    }
}
