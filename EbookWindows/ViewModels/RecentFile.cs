using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookWindows.ViewModels
{
    class RecentFile
    {
        public string fileName { get; set; }
        public string filePath { get; set; }
        public string fileIcon { get; set; }
    }

    class RecentFileDao
    {

        //ObservableCollection
        public BindingList<RecentFile> GetAll()
        {
            var list = new BindingList<RecentFile>()
                {
                    new RecentFile() { fileName="Hoàng Tử Bé", filePath="akisnkbdsac b", fileIcon="Icon/epub.png"},
                    new RecentFile() { fileName="Lâu Đài Quỷ", filePath="akisnkbdsac b", fileIcon="Icon/epub.png"},
                    new RecentFile() { fileName="Chuyện Mèo Con Dạy Hải Âu Biết Bay Mèo Con Dạy Hải Âu Biết Bay", filePath="akisnkbdsac b", fileIcon="Icon/pdf.png"},
                    new RecentFile() { fileName="Số Đỏ", filePath="akisnkbdsac b", fileIcon="Icon/epub.png"},
                    new RecentFile() { fileName="Chuyện Ngày Mưa", filePath="akisnkbdsac b", fileIcon="Icon/epub.png"},
                    new RecentFile() { fileName="Hoàng Tử Bé", filePath="akisnkbdsac b", fileIcon="Icon/epub.png"},
                    new RecentFile() { fileName="Lâu Đài Quỷ", filePath="akisnkbdsac b", fileIcon="Icon/epub.png"},
                    new RecentFile() { fileName="Chuyện Mèo Con Dạy Hải Âu Biết Bay", filePath="akisnkbdsac b", fileIcon="Icon/pdf.png"},
                    new RecentFile() { fileName="Số Đỏ", filePath="akisnkbdsac b", fileIcon="Icon/epub.png"},
                    new RecentFile() { fileName="Chuyện Ngày Mưa", filePath="akisnkbdsac b", fileIcon="Icon/epub.png"},
            };

            return list;
        }
    }
}
