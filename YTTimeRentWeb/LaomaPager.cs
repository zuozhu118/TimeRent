using System;
using System.Text;

namespace WebDemo.Common
{
    public class LaomaPager
    {
        /// <summary>
        /// ShowPageNavigate方法返回所有的分页的超链接标签以及指向的路径；
        /// </summary>
        /// <param name="pageSize">一页多少条</param>
        /// <param name="currentPage">当前页索引</param>
        /// <param name="totalCount">总条数</param>
        /// <returns></returns>
        public static string ShowPageNavigate(int pageSize, int currentPage, int totalCount,string number)
        {
            string redirectTo = "";
            
            pageSize = pageSize == 0 ? 3 : pageSize;
            var totalPages = Math.Max((totalCount + pageSize - 1) / pageSize, 1); //总页数
            var output = new StringBuilder();
            if (totalPages > 1)
            {
                if (currentPage != 1)
                {//处理首页连接
                    output.AppendFormat("<a class='pageLink' href='{0}?pageIndex=1&pageSize={1}&Number={2}' onclick='BindNavLinkClick(this.href); return false;'>首页</a> ", redirectTo, pageSize, number);

                }
                if (currentPage > 1)
                {//处理'上一页'的连接
                    output.AppendFormat("<a class='pageLink' href='{0}?pageIndex={1}&pageSize={2}&Number={3}' onclick='BindNavLinkClick(this.href); return false;'>上一页</a> ", redirectTo, currentPage - 1, pageSize, number);
                }
                else
                {
                     //output.Append("<span class='pageLink'>上一页</span>");
                }

                output.Append(" ");
                int currint = 5;
                for (int i = 0; i <= 10; i++)
                {//一共最多显示10个页码，前面5个，后面5个
                    if ((currentPage + i - currint) >= 1 && (currentPage + i - currint) <= totalPages)
                    {
                        if (currint == i)
                        {//当前页处理
                            //output.Append(string.Format("[{0}]", currentPage));
                            output.AppendFormat("<a class='cpb' href='{0}?pageIndex={1}&pageSize={2}&Number={3}' onclick='BindNavLinkClick(this.href); return false;'>{4}</a> ", redirectTo, currentPage, pageSize, number, currentPage);

                        }
                        else
                        {//一般页处理
                            output.AppendFormat("<a class='pageLink' href='{0}?pageIndex={1}&pageSize={2}&Number={3}' onclick='BindNavLinkClick(this.href); return false;'>{4}</a> ", redirectTo, currentPage + i - currint, pageSize, number, currentPage + i - currint);
                        }
                    }
                    output.Append(" ");
                }
                if (currentPage < totalPages)
                {//处理'下一页'的链接
                    output.AppendFormat("<a class='pageLink' href='{0}?pageIndex={1}&pageSize={2}&Number={3}' onclick='BindNavLinkClick(this.href); return false;'>下一页</a> ", redirectTo, currentPage + 1, pageSize, number);
                }
                else
                {
                    //output.Append("<span class='pageLink'>下一页</span>");
                }
                output.Append(" ");
                if (currentPage != totalPages)
                {
                    output.AppendFormat("<a class='pageLink' href='{0}?pageIndex={1}&pageSize={2}&Number={3}' onclick='BindNavLinkClick(this.href); return false;'>末页</a> ", redirectTo, totalPages, pageSize, number);
                }
                output.Append(" ");
            }
            output.AppendFormat("第{0}页 / 共{1}页", currentPage, totalPages);//这个统计加不加都行
            return output.ToString();
        }
    }
}