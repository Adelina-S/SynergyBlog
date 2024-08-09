namespace WebBlog
{
    public class Common
    {
        //Разделяет строгу с тэгами на отдельные тэги
        public static string SplitTags(string tags, out List<string> result)
        {
            string answer = "";
            result= new List<string>();
            if (string.IsNullOrEmpty(tags)) return answer;
            //Удаляет нулевые символы
            tags= tags.Trim();
            if (tags.Length==0) return answer;
            if (tags[0] != '#') answer = "Тэги должны начинаться со знака решётки;";
            //разделяет по символу тэга
            tags = tags.Replace(",", "");
            tags = tags.Replace(";", "");
            result = tags.Split('#').ToList();
            //удаляет пустые символы и удаляет нулевые строки
            result = result.Select(t => t.Trim()).Where(t => t.Length > 0).ToList();
            if (result.Where(t => t.Length < 2).Count() > 0) answer+= " Недопустимы тэги длиной в один символ;";
            return answer;
        }
    }
}
