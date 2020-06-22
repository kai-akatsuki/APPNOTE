using APPNOTE_dbfirst.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using APPNOTE_dbfirst.Controllers;
using System.Runtime.CompilerServices;
using System.CodeDom.Compiler;

namespace APPNOTE_dbfirst
{
    public partial class frmMain : Form
    {
        //mode = true, chuong trinh dang o che do All Notes
        //mode = false, chuong trinh dang o che do Trash
        bool mode;

        //list iptemp co stt la so cot cua listview lv, gia tri tuong ung la Id cua note hien tren listview lv
        //dung de tim note.Id de de dang trong viec truy van
        List<int> idtemp = new List<int>();

        //chuong trinh khoi dong voi che do All Notes
        public frmMain()
        {
            InitializeComponent();
            this.rtb1.ReadOnly = true;
            mode = true;
            this.cTittle.Text = "All Notes";
            this.btnNewNote.Visible = true;
            this.btnDelete.Visible = true;
            this.btnPin.Visible = true;
            this.toolStrip3.Visible = true;
            this.rtb.ReadOnly = false;
            this.btnRestore.Visible = false;
            this.btnDeleteForever.Visible = false;
            this.lbInfo.Text = "";
            displayAllNotes();
        }

        //hien thi danh sach note trong All Notes theo note.Tittle tren listview lv
        //dong thoi cap nhat idtemp
        private void displayAllNotes()
        {
            idtemp.Clear();
            this.lv.Items.Clear();
            List<Note> notes = NoteControllers.getAllNotes();

            //hien thi cac note duoc pin truoc
            for (int i = (notes.Count() - 1); i >= 0; i--)
            {
                Note note = notes[i];
                if (note.isTrash == false && note.isPin == true)
                {
                    ListViewItem eVent = new ListViewItem(note.Tittle.Trim());
                    eVent.SubItems.Add(new ListViewItem.ListViewSubItem(eVent, "x"));
                    this.lv.Items.Add(eVent);
                    idtemp.Add(note.Id);
                }
            }

            //cac note khong duoc pin
            for (int i = (notes.Count() - 1); i >= 0; i--)
            {
                Note note = notes[i];
                if (note.isTrash == false && note.isPin == false)
                {
                    ListViewItem eVent = new ListViewItem(note.Tittle.Trim());
                    eVent.SubItems.Add(new ListViewItem.ListViewSubItem(eVent, ""));
                    this.lv.Items.Add(eVent);
                    idtemp.Add(note.Id);
                }
            }
        }
        //hien thi danh sach note trong Trash theo note.Tittle tren listview lv
        //dong thoi cap nhat idtemp
        private void displayTrash()
        {
            idtemp.Clear();
            this.lv.Items.Clear();
            List<Note> notes = NoteControllers.getAllNotes();
            for (int i = (notes.Count() - 1); i >= 0; i--)
            {
                Note note = notes[i];
                if (note.isTrash == true)
                {
                    ListViewItem eVent = new ListViewItem(note.Tittle.Trim());
                    eVent.SubItems.Add(new ListViewItem.ListViewSubItem(eVent, ""));
                    this.lv.Items.Add(eVent);
                    idtemp.Add(note.Id);
                }
            }
        }

        //chuyen doi giao dien giua 2 che do: All notes va Trash
        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.lv.SelectedIndices.Clear();
            this.rtb.Clear();
            this.rtb1.Clear();
            this.txbFind.Text = "";

            //neu dang o che do All Notes, chuyen sang che do Trash
            if (mode == true)
            {
                mode = false;
                this.cTittle.Text = "Trash";
                this.btnNewNote.Visible = false;
                this.btnDelete.Visible = false;
                this.btnPin.Visible = false;
                this.toolStrip3.Visible = false;
                this.rtb.ReadOnly = true;
                this.btnRestore.Visible = true;
                this.btnDeleteForever.Visible = true;
                this.lbInfo.Text = "";
                displayTrash();
            }
            //nguoc lai
            else
            {
                mode = true;
                this.cTittle.Text = "All Notes";
                this.btnNewNote.Visible = true;
                this.btnDelete.Visible = true;
                this.btnPin.Visible = true;
                this.toolStrip3.Visible = true;
                this.rtb.ReadOnly = false;
                this.btnRestore.Visible = false;
                this.btnDeleteForever.Visible = false;
                this.lbInfo.Text = "";
                displayAllNotes();
            }
        }

        //tao note moi
        private void btnNewNote_Click(object sender, EventArgs e)
        {
            //neu o che do Trash, khong the tao note moi
            if (mode == false)
            {
                return;
            }

            this.lv.SelectedIndices.Clear();
            this.lbInfo.Text = "";
            this.rtb.Clear();
            this.rtb1.Clear();
            this.txbFind.Text = "";

            //tao note moi voi note.content trong truoc
            //nguoi dung se thay doi noi dung bang cach viet vao richtextbox rtb
            Note note = new Note();
            note.Id = NoteControllers.getIdFromDb();
            note.Tittle = "New note";
            note.Content = "";
            note.Info = DateTime.Now;
            note.isTrash = false;
            note.isPin = false;

            NoteControllers.AddNote(note);
            displayAllNotes();

            int i = 0;
            while (i < idtemp.Count)
            {
                if (idtemp[i] == note.Id)
                {
                    break;
                }
                i++;
            }
            this.lv.Items[i].Selected = true;
            this.lv.Select();
            this.rtb.Text = NoteControllers.getANote(note.Id).Content;
            this.rtb.Focus();
        }

        //chuyen note vao Trash, note.isTrash chuyen tu false sang true
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                int i = this.lv.SelectedItems[0].Index;
                int id = idtemp[i];
                Note note = NoteControllers.getANote(id);
                note.isTrash = true;

                NoteControllers.UpdateNote(note);

                displayAllNotes();

                this.rtb.Clear();
                this.rtb1.Clear();
                this.lbInfo.Text = "";

            }
            catch
            {

            }
        }

        //khoi phuc note tu Trash tro lai All Notes, note.isTrash chuyen tu true sang false
        private void btnRestore_Click(object sender, EventArgs e)
        {
            try
            {
                int i = this.lv.SelectedItems[0].Index;
                int id = idtemp[i];
                Note note = NoteControllers.getANote(id);
                note.isTrash = false;

                NoteControllers.UpdateNote(note);
                displayTrash();

                this.rtb.Clear();
                this.rtb1.Clear();
                this.lbInfo.Text = "";
            }
            catch
            {

            }
        }

        //Xóa vĩnh viễn note
        private void btnDeleteForever_Click(object sender, EventArgs e)
        {
            try
            {
                int i = this.lv.SelectedItems[0].Index;
                int id = idtemp[i];
                Note note = NoteControllers.getANote(id);

                NoteControllers.DeleteNote(note);
                displayTrash();

                //xoa cac tag khong su dung
                List<Tag> tags = TagControllers.getAllTags();
                foreach (Tag tag in tags)
                {
                    if (tag.Note.Count() <= 0)
                    {
                        TagControllers.DeleteTag(tag);
                    }
                }

                this.rtb.Clear();
                this.rtb1.Clear();
                this.lbInfo.Text = "";
            }
            catch
            {

            }

        }

        //hien thi hoac tat hien thi thoi diem cap nhat cuoi cung cua note
        private void btnInfo_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.lbInfo.Text == "")
                {
                    int i = this.lv.SelectedItems[0].Index;
                    int id = idtemp[i];
                    Note note = NoteControllers.getANote(id);

                    this.lbInfo.Text = "Modified: " + note.Info.ToString().Trim();
                }
                else
                {
                    this.lbInfo.Text = "";
                }
            }
            catch
            {

            }
        }

        //chon note bang cach click vao tieu de cua note tren listview lv
        //note.content se hien len richtextbox rtb va note.Tags se hien len richtetbox rtb1
        //dong thoi co the su dung cac nut chuc nang de cap nhat note voi note duoc chon
        private void lv_Click(object sender, EventArgs e)
        {
            try
            {
                this.lbInfo.Text = "";
                int i = this.lv.SelectedItems[0].Index;
                int id = idtemp[i];

                if (id == -1)
                {
                    this.lv.SelectedIndices.Clear();
                    return;
                }

                Note note = NoteControllers.getANote(id);

                this.rtb.Text = note.Content;

                this.rtb1.Clear();
                if (note.Tag.Count <= 0) { return; }
                string displayTags = "";
                foreach (Tag tag in note.Tag)
                {
                    displayTags = displayTags + tag.tagname + " ";
                }
                this.rtb1.Text = displayTags;
            }
            catch
            {

            }
        }

        //Thêm tag vao note
        //tag.tagname nhap o textbox txbAddOrFindTag
        private void btnAddTag_Click(object sender, EventArgs e)
        {
            try
            {
                //txbAddOrFindTag rong hoac chua chon note de add tag, thoat
                if (this.txbAddOrFindTag.Text == "" || this.lv.SelectedItems.Count <= 0)
                {
                    return;
                }

                List<Tag> tags1 = TagControllers.getAllTags();
                foreach (Tag tag1 in tags1)
                {
                    //kiem tra tag ton tai chua
                    if (this.txbAddOrFindTag.Text == tag1.tagname)
                    {
                        goto b2;
                    }
                }

                //tao tag moi neu tag chua ton tai
                Tag tag2 = new Tag();
                tag2.tagname = this.txbAddOrFindTag.Text;
                TagControllers.AddTag(tag2);
                goto b3;

            //neu tag da duoc gan cho note truoc do roi, thoat
            b2:
                int i1 = this.lv.SelectedItems[0].Index;
                int id1 = idtemp[i1];
                Note note1 = NoteControllers.getANote(id1);
                foreach (Tag tag3 in note1.Tag)
                {
                    if (this.txbAddOrFindTag.Text == tag3.tagname)
                    {
                        this.txbAddOrFindTag.Text = "";
                        return;
                    }
                }
                goto b3;

            //gan tag cho note va cap nhat note
            b3:
                Tag tag4 = TagControllers.getATag(this.txbAddOrFindTag.Text);
                int i2 = this.lv.SelectedItems[0].Index;
                int id2 = idtemp[i2];
                Note note2 = NoteControllers.getANote(id2);
                note2.Tag.Add(tag4);

                Note note3 = new Note();
                note3.Id = note2.Id;
                note3.Tittle = note2.Tittle;
                note3.Content = note2.Content;
                note3.Info = note2.Info;
                note3.isTrash = note2.isTrash;
                note3.isPin = note2.isPin;
                foreach (Tag tag5 in note2.Tag)
                {
                    note3.Tag.Add(tag5);
                }

                NoteControllers.DeleteNote(note2);
                NoteControllers.AddNote(note2);

                string displayTags = "";
                foreach (Tag tag in note2.Tag)
                {
                    displayTags = displayTags + tag.tagname + " ";
                }
                this.rtb1.Text = displayTags;

                this.txbAddOrFindTag.Text = "";
            }
            catch
            {

            }
        }

        //pin note len tren dau, note.isPin chuyen tu false sang true
        //nguoc lai, note.isPin chuyen tu true sang false
        //neu co nhieu note duoc pin se sap xep theo thu tu
        private void btnPin_Click(object sender, EventArgs e)
        {
            try
            {
                int i = this.lv.SelectedItems[0].Index;
                int id = idtemp[i];
                Note note = NoteControllers.getANote(id);
                if (note.isPin == false)
                {
                    note.isPin = true;
                }
                else
                {
                    note.isPin = false;
                }

                NoteControllers.UpdateNote(note);
                displayAllNotes();

                int j = 0;
                while (j < idtemp.Count)
                {
                    if (idtemp[j] == note.Id)
                    {
                        break;
                    }
                    j++;
                }
                this.lv.Items[j].Selected = true;
                this.lv.Select();
            }
            catch
            {

            }
        }

        //lay vi tri xuong dong dau tien, dung trong viec lay dong dau tien lam note.Tittle
        private int getfirstline(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i].ToString() == "\n")
                {
                    return i;
                }
            }
            return str.Length;
        }

        //viet hoac thay doi note.content khi o che do All Notes
        //khong the lam dieu tuong tu khi o che do Trash
        private void rtb_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (mode == true)
                {
                    int i = this.lv.SelectedItems[0].Index;
                    int id = idtemp[i];
                    Note note = NoteControllers.getANote(id);

                    string old_Content = note.Content;

                    note.Content = this.rtb.Text.ToString();

                    note.Tittle = note.Content.Substring(0, getfirstline(note.Content));
                    if (note.Content != old_Content)
                    {
                        this.lbInfo.Text = "";
                        note.Info = DateTime.Now;
                    }
                    if (note.Content == "")
                    {
                        note.Tittle = "New note";
                    }

                    NoteControllers.UpdateNote(note);

                    this.lv.SelectedItems[0].Text = note.Tittle.ToString();
                }
                else
                {
                    return;
                }
            }
            catch
            {

            }
        }

        //thoat chuong trinh
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //tim kiem note, theo noi dung va theo tag
        private void txbFind_TextChanged(object sender, EventArgs e)
        {
            this.lv.SelectedIndices.Clear();
            this.lbInfo.Text = "";
            this.rtb.Clear();
            this.rtb1.Clear();

            if (this.txbFind.Text == "")
            {
                if (mode == true)
                {
                    displayAllNotes();
                }
                else
                {
                    displayTrash();
                }
                return;
            }

            this.lv.Items.Clear();
            List<Note> notes = NoteControllers.getAllNotes(this.txbFind.Text);
            List<Tag> tags = TagControllers.getAllTags(this.txbFind.Text);

            //khi o che do All Notes
            if (mode == true)
            {

                idtemp.Clear();

                //tim kiem theo noi dung
                ListViewItem eVent2 = new ListViewItem("### Search by content:");
                this.lv.Items.Add(eVent2);
                idtemp.Add(-1);

                //note duoc pin hien truoc
                foreach (Note note in notes)
                {
                    if (note.isTrash == false && note.isPin == true)
                    {
                        ListViewItem eVent = new ListViewItem(note.Tittle.Trim());
                        eVent.SubItems.Add(new ListViewItem.ListViewSubItem(eVent, "x"));
                        this.lv.Items.Add(eVent);
                        idtemp.Add(note.Id);
                    }
                }
                //note khong duoc pin
                foreach (Note note in notes)
                {
                    if (note.isTrash == false && note.isPin == false)
                    {
                        ListViewItem eVent = new ListViewItem(note.Tittle.Trim());
                        eVent.SubItems.Add(new ListViewItem.ListViewSubItem(eVent, ""));
                        this.lv.Items.Add(eVent);
                        idtemp.Add(note.Id);
                    }
                }

                //tim kiem theo tag
                ListViewItem eVent1 = new ListViewItem("### Search by tag:");
                this.lv.Items.Add(eVent1);
                idtemp.Add(-1);

                foreach (Tag tag in tags)
                {
                    //note duoc pin hien truoc
                    foreach (Note note in tag.Note)
                    {
                        if (note.isTrash == false && note.isPin == true)
                        {
                            ListViewItem eVent = new ListViewItem(note.Tittle.Trim());
                            eVent.SubItems.Add(new ListViewItem.ListViewSubItem(eVent, "x"));
                            this.lv.Items.Add(eVent);
                            idtemp.Add(note.Id);
                        }
                    }
                    //note khong duoc pin
                    foreach (Note note in tag.Note)
                    {
                        if (note.isTrash == false && note.isPin == false)
                        {
                            ListViewItem eVent = new ListViewItem(note.Tittle.Trim());
                            eVent.SubItems.Add(new ListViewItem.ListViewSubItem(eVent, ""));
                            this.lv.Items.Add(eVent);
                            idtemp.Add(note.Id);
                        }
                    }
                }
            }

            //khi o che do Trash
            if (mode == false)
            {
                idtemp.Clear();

                //tim kiem theo noi dung
                ListViewItem eVent2 = new ListViewItem("### Search by content:");
                this.lv.Items.Add(eVent2);
                idtemp.Add(-1);
                foreach (Note note in notes)
                {
                    if (note.isTrash == true)
                    {
                        ListViewItem eVent = new ListViewItem(note.Tittle.Trim());
                        eVent.SubItems.Add(new ListViewItem.ListViewSubItem(eVent, ""));
                        this.lv.Items.Add(eVent);
                        idtemp.Add(note.Id);
                    }
                }

                //tim kiem theo tag
                ListViewItem eVent1 = new ListViewItem("### Search from tag:");
                this.lv.Items.Add(eVent1);
                idtemp.Add(-1);
                foreach (Tag tag in tags)
                {
                    foreach (Note note in tag.Note)
                    {
                        if (note.isTrash == true)
                        {
                            ListViewItem eVent = new ListViewItem(note.Tittle.Trim());
                            eVent.SubItems.Add(new ListViewItem.ListViewSubItem(eVent, ""));
                            this.lv.Items.Add(eVent);
                            idtemp.Add(note.Id);
                        }
                    }
                }
            }
        }

        //xoa tag
        //tag.tagname nhap o textbox txbAddOrFindTag
        private void btnDeleteTag_Click(object sender, EventArgs e)
        {
            try
            {
                if (txbAddOrFindTag.Text == "" || this.lv.SelectedItems.Count <= 0)
                {
                    return;
                }
                int i = this.lv.SelectedItems[0].Index;
                int id = idtemp[i];
                Note note = NoteControllers.getANote(id);
                foreach (Tag tag in note.Tag)
                {
                    //kiem tra tag da duoc gan cho note chua
                    if (tag.tagname == this.txbAddOrFindTag.Text)
                    {
                        //xoa tag khoi note.Tags va cap nhat note
                        note.Tag.Remove(tag);

                        Note note1 = new Note();
                        note1.Id = note.Id;
                        note1.Tittle = note.Tittle;
                        note1.Content = note.Content;
                        note1.Info = note.Info;
                        note1.isTrash = note.isTrash;
                        note1.isPin = note.isPin;
                        foreach (Tag tag1 in note.Tag)
                        {
                            note1.Tag.Add(tag1);
                        }

                        NoteControllers.DeleteNote(note);
                        NoteControllers.AddNote(note1);

                        string displayTags = "";
                        foreach (Tag tag2 in note1.Tag)
                        {
                            displayTags = displayTags + tag2.tagname + " ";
                        }
                        this.rtb1.Text = displayTags;

                        this.txbAddOrFindTag.Text = "";

                        //xoa tag khong su dung
                        List<Tag> tags = TagControllers.getAllTags();
                        foreach (Tag tag3 in tags)
                        {
                            if (tag3.Note.Count() <= 0)
                            {
                                TagControllers.DeleteTag(tag3);
                            }
                        }

                        return;
                    }
                }
                return;
            }
            catch
            {

            }
        }
        //ngan nguoi dung thay doi kich thuoc cot listview lv 
        private void lv_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = lv.Columns[e.ColumnIndex].Width;
        }
    }
}
