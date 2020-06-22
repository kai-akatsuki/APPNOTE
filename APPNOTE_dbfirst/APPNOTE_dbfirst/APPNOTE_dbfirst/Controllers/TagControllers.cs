using APPNOTE_dbfirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPNOTE_dbfirst.Controllers
{
    public class TagControllers
    {
        //them tag
        public static bool AddTag(Tag tag)
        {
            using (var _context = new APPNOTEEntities())
            {
                _context.Tag.Add(tag);
                _context.SaveChanges();
                return true;
            }
        }

        //xoa tag
        public static bool DeleteTag(Tag tag)
        {
            using (var _context = new APPNOTEEntities())
            {
                var dbtag = (from u in _context.Tag
                             where u.tagname == tag.tagname
                             select u).SingleOrDefault();
                foreach (var note in dbtag.Note)
                {
                    foreach (var u in note.Tag)
                    {
                        if (u.tagname == tag.tagname)
                        {
                            note.Tag.Remove(u);
                            break;
                        }
                    }
                }
                _context.Tag.Remove(dbtag);
                _context.SaveChanges();
                return true;
            }
        }

        //lay tag theo tagname
        public static Tag getATag(string tagname)
        {
            using (var _context = new APPNOTEEntities())
            {
                var tag = (from u in _context.Tag
                           where u.tagname == tagname
                           select u).ToList();
                if (tag.Count == 1)
                {
                    return tag[0];
                }
                else
                {
                    return null;
                }
            }
        }

        //lay list tat ca tag
        public static List<Tag> getAllTags()
        {
            using (var _context = new APPNOTEEntities())
            {
                var tags = (from u in _context.Tag.AsEnumerable()
                             select new
                             {
                                 tagname = u.tagname,
                                 Notes = u.Note
                             })
                            .Select(x => new Tag
                            {
                                tagname = x.tagname,
                                Note = x.Notes
                            }).ToList();
                return tags;
            }
        }

        //lay list tag theo chuoi tim kiem searchstr
        public static List<Tag> getAllTags(string searchtr)
        {
            using (var _context = new APPNOTEEntities())
            {
                var tags = (from u in _context.Tag.AsEnumerable()
                            where u.tagname.Contains(searchtr)
                            select new
                            {
                                tagname=u.tagname,
                                Notes=u.Note
                            })
                            .Select(x => new Tag
                            {
                                tagname=x.tagname,
                                Note=x.Notes
                            }).ToList();
                return tags;
            }
        }
    }
}
