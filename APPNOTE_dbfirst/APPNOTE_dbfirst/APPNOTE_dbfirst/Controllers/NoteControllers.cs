using APPNOTE_dbfirst.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Deployment.Application;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPNOTE_dbfirst.Controllers
{
    public class NoteControllers
    {
        //lay list tat ca note tu database len
        public static List<Note> getAllNotes()
        {
            using (var _context = new APPNOTEEntities())
            {
                var notes = (from u in _context.Note.AsEnumerable()
                             select new
                             {
                                 Id = u.Id,
                                 Tittle = u.Tittle,
                                 Content = u.Content,
                                 Info = u.Info,
                                 isTrash = u.isTrash,
                                 isPin = u.isPin,
                                 Tags = u.Tag
                             })
                            .Select(x => new Note
                            {
                                Id = x.Id,
                                Tittle = x.Tittle,
                                Content = x.Content,
                                Info = x.Info,
                                isTrash = x.isTrash,
                                isPin = x.isPin,
                                Tag = x.Tags
                            }).ToList();
                return notes;
            }
        }

        //lay 1 note theo id
        public static Note getANote(int id)
        {
            using (var _context = new APPNOTEEntities())
            {
                var note = (from u in (from i in _context.Note.AsEnumerable()
                                       select i)
                            where u.Id == id
                            select new
                            {
                                Id = u.Id,
                                Tittle = u.Tittle,
                                Content = u.Content,
                                Info = u.Info,
                                isTrash = u.isTrash,
                                isPin = u.isPin,
                                Tags = u.Tag
                            })
                            .Select(x => new Note
                            {
                                Id = x.Id,
                                Tittle = x.Tittle,
                                Content = x.Content,
                                Info = x.Info,
                                isTrash = x.isTrash,
                                isPin = x.isPin,
                                Tag = x.Tags
                            }).SingleOrDefault();
                return note;
            }
        }

        //lay Id tiep theo
        public static int getIdFromDb()
        {
            using (var _context = new APPNOTEEntities())
            {
                var id = (from t in _context.Note
                          select t.Id).ToList();
                if (id.Count <= 0)
                {
                    return 1;
                }
                return id.Max() + 1;
            }
        }

        //them note
        public static bool AddNote(Note note)
        {
            try
            {
                using (var _context = new APPNOTEEntities())
                {
                    foreach (var tag in note.Tag)
                    {
                        var tagdb = (from u in _context.Tag
                                     where u.tagname == tag.tagname
                                     select u).Single();
                        tagdb.Note.Add(note);
                    }
                    note.Tag.Clear();
                    _context.Note.Add(note);
                    _context.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        //cap nhat note
        public static bool UpdateNote(Note note)
        {
            using (var _context = new APPNOTEEntities())
            {
                _context.Note.AddOrUpdate(note);
                _context.SaveChanges();
                return true;
            }
        }

        //xoa note
        public static bool DeleteNote(Note note)
        {
            using (var _context = new APPNOTEEntities())
            {
                var dbnote = (from u in _context.Note
                              where u.Id == note.Id
                              select u).SingleOrDefault();
                foreach (var tag in dbnote.Tag)
                {
                    foreach (var u in tag.Note)
                    {
                        if (u.Id == note.Id)
                        {
                            tag.Note.Remove(u);
                            break;
                        }
                    }
                }
                _context.Note.Remove(dbnote);
                _context.SaveChanges();
                return true;
            }
        }

        //lay list tat ca note theo 1 chuoi tim kiem searchstr
        public static List<Note> getAllNotes(string searchstr)
        {
            using (var _context = new APPNOTEEntities())
            {
                var notes = (from u in _context.Note.AsEnumerable()
                             where u.Content.Contains(searchstr)
                             select new
                             {
                                 Id = u.Id,
                                 Tittle = u.Tittle,
                                 Content = u.Content,
                                 Info = u.Info,
                                 isTrash = u.isTrash,
                                 isPin = u.isPin,
                                 Tags = u.Tag
                             })
                            .Select(x => new Note
                            {
                                Id = x.Id,
                                Tittle = x.Tittle,
                                Content = x.Content,
                                Info = x.Info,
                                isTrash = x.isTrash,
                                isPin = x.isPin,
                                Tag = x.Tags
                            }).ToList();
                return notes;
            }
        }
    } 
}
