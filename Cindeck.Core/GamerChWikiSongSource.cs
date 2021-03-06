﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    public class GamerChWikiSongSource
    {
        private IDocumentSource m_doc;
        private const int TypeColumn = 0, TitleColumn = 1, DifficultyColumn = 3, LevelColumn = 4, NotesColumn = 5, DurationColumn = 6;

        public GamerChWikiSongSource(IDocumentSource document)
        {
            m_doc = document;
        }

        public async Task<Tuple<List<Song>, int>> GetSongs()
        {
            var raw = await m_doc.Load();
            var hdoc = new HtmlDocument();
            hdoc.LoadHtml(raw);
            var rows = hdoc.DocumentNode.SelectNodes("//article/section/table/tbody/tr");
            var songs = new Dictionary<string, Song>();
            int failed = 0;
            foreach(var row in rows)
            {
                try
                {
                    var columns =row.SelectNodes("td");
                    var duration = columns[DurationColumn].ChildNodes.First(x => x.Name == "#text").InnerText.Trim().Split(':');
                    var title = string.Join(" ",
                        columns[TitleColumn].InnerText.Trim()
                        .Split(new[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())
                        .ToArray());
                    if (duration.Length < 2)
                    {
                        // まだプレイ時間のデータがない場合は2分と想定する
                        duration = new[] { "2", "00" };
                    }
                    var data = new SongData(columns[DifficultyColumn].ChildNodes.First(x=>x.Name=="#text").InnerText.Trim().ToSongDifficulty(),
                                        int.Parse(columns[LevelColumn].ChildNodes.First(x => x.Name == "#text").InnerText.Trim()),
                                        int.Parse(columns[NotesColumn].ChildNodes.First(x => x.Name == "#text").InnerText.Trim()),
                                        int.Parse(duration[0])*60+int.Parse(duration[1]));
                    
                    var type = columns[TypeColumn].ChildNodes.First(x => x.Name == "#text").InnerText.Trim().ToSongType();

                    if (!songs.ContainsKey(title))
                    {
                        songs[title] = new Song(title, type);
                    }
                    songs[title].Data[data.Difficulty] = data;
                }
                catch(Exception)
                {
                    failed++;
                    continue;
                }
            }
            return Tuple.Create(songs.Values.OrderBy(x => x.Title).ToList(), failed);
        }
    }
}
