using Discord;
using System.Text;
using System.Linq;
using System.Linq.Expressions;

namespace SovereigntyBot.Services
{
    public struct Header
    {
        public string Title;
        public bool Locked;
    }
    public class Table
    {
        public Header[] Headers;
        public int[] Fields; // contains index of headers
        private int[] _presetFields;
        public int[] PresetFields {
            get {
                return _presetFields;
            }

            set {
                _presetFields = value;
                Fields = value; 
            }
        }
        public List<object[]> Content = new List<object[]>();
        private int _startRowIndex { get; set; } = 0;
        private int _endRow { get; set; } = 0;
        private int _totalRowDisplay { get; set; } = 20;
        // private ulong _messageId { get; set; }

        public Table(params string[] headerTitles)
        {
            Header[] headers = new Header[headerTitles.Length];
            for(int i=0; i < headerTitles.Length; i++)
            {
                var header = new Header();
                header.Title = headerTitles[i];
                header.Locked = false;
                headers[i] = header;
            }
            Headers = headers;
            PresetFields = DefaultPresetFields();
        }

        public void AddRow(params object[] rowData)
        {
            if(rowData.Length != Headers.Length)
            {
                Program.Log(new LogMessage(LogSeverity.Error, "Table.cs", "Argument does not match with Headers length"));
                throw new ArgumentException("Argument does not match with Headers length");
            }
            Content.Add(rowData);
            _endRow = Content.Count < _totalRowDisplay ? Content.Count : _startRowIndex + _totalRowDisplay;
        }

        public int[] DefaultPresetFields()
        {   
            // supports up to 3 maximum headers
            int currentHeadersLength = Headers.Length > 3 ? 3 : Headers.Length;

            int[] currentHeaders = new int[currentHeadersLength];

            for(int i=0; i < currentHeaders.Length; i++)
            {
                currentHeaders[i] = i;
            }

            return currentHeaders;
        }

        public void Reset()
        {
            foreach(int columnIndex in Fields)
            {
                Headers[columnIndex].Locked = false;
            }
            Fields = PresetFields;
            _startRowIndex = 0;
            _endRow = _startRowIndex + _totalRowDisplay;
        }

        public void SetLock(bool Lock, int fieldIndex)
        {
            Headers[Fields[fieldIndex]].Locked = Lock;
        }

        public int FreeHeadersAhead(int[] fields, int headerIndex, int direction)
        {  
            int FreeHeadersAhead = 0;

            for(int i=headerIndex + direction; i < Headers.Length && i >= 0; i += direction) // need testing
            {
                Header headerToCheck = Headers[i];
                FreeHeadersAhead++;
                if(headerToCheck.Locked == true || fields.Contains(i)) // check if Field[index] has header
                {
                    continue;
                }
                return FreeHeadersAhead;
            }
            return 0;
        }

        public int GetTotalLockedFields()
        {
            int totalFieldsLocked = 0;
            foreach(int hIndex in Fields)
            {
                if(Headers[hIndex].Locked == true)
                {
                    totalFieldsLocked++;
                }
            }
            return totalFieldsLocked;
        }

        public bool RotateHorizontal(int direction, bool isSimulated)
        {
            if(direction != 1 && direction != -1) throw new ArgumentException("Direction argument is invalid");
            
            bool success = false;
            int[] newFields = new int[Fields.Length];

            for(int i=0; i < newFields.Length; i++)
            {
                newFields[i] = Fields[i];
            }

            int starterFieldIndex = direction == 1 ? newFields.Length - 1 : 0;
            int fieldDirection = direction * -1;
            for(int i = starterFieldIndex; i < newFields.Length && i >= 0; i += fieldDirection)
            {
                int headerIndex = newFields[i];
                if(Headers[headerIndex].Locked == true)
                {
                    continue;
                }
                // if next headerindex is valid
                int freeHeadersAhead = FreeHeadersAhead(newFields, headerIndex, direction) * direction;

                if(freeHeadersAhead == 0 )
                {
                    continue;
                }
                newFields[i] = headerIndex + freeHeadersAhead;
                success = true;
            }

            if(isSimulated == false)
            { // edited fields even when simulated, cuz newFields might be a reference to Fields
                Fields = newFields;
            }
            return success;
        }

        public bool RotateVertical(int direction, bool isSimulated)
        {
            if(direction != 1 && direction != -1) throw new ArgumentException("Direction is invalid");
            bool success = false; // somehow startRowIndex and endRow is the same,
            if(
                (_startRowIndex <= 0 && direction == -1) || 
                (_endRow >= Content.Count && direction == 1)
            )
            {
                return success;
            }
            // updated start row index = 20
            int updatedStartRowIndex = _startRowIndex + _totalRowDisplay * direction;
            if(updatedStartRowIndex < 0 ) { updatedStartRowIndex = 0; }

            int updatedEndRow = updatedStartRowIndex + _totalRowDisplay; // 40
            if(updatedStartRowIndex >= Content.Count || updatedEndRow >= Content.Count)
            {
                updatedEndRow = Content.Count;
                updatedStartRowIndex = _endRow;
            }

            if(updatedEndRow <= -1 || updatedStartRowIndex >= Content.Count) { return success; }
            
            success = true;
            if(isSimulated == false)
            {
                _startRowIndex = updatedStartRowIndex;
                _endRow = updatedEndRow;
            }

            return success;
        }

        public string DisplayValues(int columnIndex)
        {
            StringBuilder sb = new();
            try
            {
                if(Fields[columnIndex] > Content[0].Length - 1)
                {
                    // HeaderIndex is larger than content's length
                    return "N/A";
                }
            }
            catch(ArgumentOutOfRangeException)
            {
                // table contains no content
                return "N/A";
            }

            for(int rowIndex = _startRowIndex; rowIndex < _endRow; rowIndex++)
            {
                object[] rowData = Content[rowIndex];
                object data = rowData[Fields[columnIndex]];
                sb.Append(data == null ? "N/A\n" : data.ToString() + "\n");
            }

            return sb.ToString();
        }

        public List<EmbedFieldBuilder> GetDisplayFields()
        {
            List<EmbedFieldBuilder> displayFields = new();
            for(int columnIndex = 0; columnIndex < Fields.Length; columnIndex++)
            {
                string data = DisplayValues(columnIndex);
                string headerName = Headers[Fields[columnIndex]].Title;
                string updatedHeaderName = Headers[Fields[columnIndex]].Locked == true ? headerName + " 🔒" : headerName;

                displayFields.Add(new EmbedFieldBuilder().WithName(updatedHeaderName).WithValue(data).WithIsInline(true));
            }
            return displayFields;
        }

        private string ArrayToString(int[] array)
        {
            return string.Join(", ", array);
        }

        public ButtonBuilder GetControlPanelButton()
            => new ButtonBuilder(emote: new Emoji("🔍"), customId: "tcp-get", style: ButtonStyle.Primary);

        public ComponentBuilder GetControlPanel()
        {
            // Back And Reset Buttons
            var resetButton = new ButtonBuilder(emote: new Emoji("🔄"), customId: "tcp-reset", style: ButtonStyle.Danger);
            var backButton = new ButtonBuilder(emote: new Emoji("⬅️"), customId: "tcp-back", style: ButtonStyle.Danger);

            // Rotation Buttons 

            bool leftSuccess = RotateHorizontal(-1, true);
            bool rightSuccess = RotateHorizontal(1, true);
            bool upSuccess = RotateVertical(-1, true);
            bool downSuccess = RotateVertical(1, true);

            var rotateRightButton = new ButtonBuilder(emote: new Emoji("▶️"), customId: "tcp-rotate-right", isDisabled: !rightSuccess);
            var rotateLeftButton = new ButtonBuilder(emote: new Emoji("◀️"), customId: "tcp-rotate-left", isDisabled: !leftSuccess);
            var rotateUpButton = new ButtonBuilder(emote: new Emoji("🔼"), customId: "tcp-rotate-up", isDisabled: !upSuccess);
            var rotateDownButton = new ButtonBuilder(emote: new Emoji("🔽"), customId: "tcp-rotate-down", isDisabled: !downSuccess);

            // Lock Buttons

            // no precautions for tables with fields less than 3
            ButtonBuilder[] lockButtons = new ButtonBuilder[Fields.Length];
            // make a method to attempt to get lock buttons, if not possible, return a disabled button

            for(int fieldIndex=0; fieldIndex < Fields.Length; fieldIndex++)
            {
                bool isFieldLocked = Headers[Fields[fieldIndex]].Locked; // index out of range
                // string headerName = Headers[Fields[fieldIndex]].Title;
                Emoji emojiLocked = isFieldLocked == true ? new Emoji("🔓") : new Emoji("🔒");
                string Field1CustomId = isFieldLocked == true ? $"tcp-unlock-field-{fieldIndex + 1}" : $"tcp-lock-field-{fieldIndex + 1}";
                ButtonStyle FieldStyle = isFieldLocked == true ? ButtonStyle.Secondary : ButtonStyle.Primary;
                lockButtons[fieldIndex] = new ButtonBuilder(customId: Field1CustomId, style: FieldStyle, emote: emojiLocked);
            }

            // Rows

            ActionRowBuilder firstRow = new ActionRowBuilder();
            ActionRowBuilder secondRow = new ActionRowBuilder();
            ActionRowBuilder thirdRow = new ActionRowBuilder();

            firstRow.WithButton(backButton).WithButton(rotateUpButton).WithButton(resetButton);
            secondRow.WithButton(rotateLeftButton).WithButton(GetLockButton(lockButtons, 1)).WithButton(rotateRightButton);
            thirdRow.WithButton(GetLockButton(lockButtons, 0)).WithButton(rotateDownButton).WithButton(GetLockButton(lockButtons, 2));


            var wComponent = new ComponentBuilder()
                .WithRows(new List<ActionRowBuilder>(){firstRow, secondRow, thirdRow});
            
            return wComponent;
        }

        private static ButtonBuilder GetLockButton(ButtonBuilder[] lockButtons, int index)
        {
            try{
                return lockButtons[index];
            }
            catch(IndexOutOfRangeException)
            {
                return new ButtonBuilder(customId: $"tcp-disabled-{index}", style: ButtonStyle.Secondary, emote: new Emoji("🚫"), isDisabled: true);
            }
        }
    }

    public static class TableExtensions
    {
        public static Table WithPresetFields(this Table table, int[] presetFields)
        {
            if(presetFields.Length != table.Fields.Length)
            {
                Program.Log(new LogMessage(LogSeverity.Error, "Table.cs", "Preset Fields cannot be set"));
                return table;
            }
            table.PresetFields = presetFields;
            return table;
        }

        public static Table WithFilter(this Table table, int fieldIndex, Func<object, bool> function)
        {
            List<object[]> newTableContent = new();

            foreach(object[] rowData in table.Content)
            {
                bool filter = function(rowData[fieldIndex]);
                if(filter == true)
                {
                    newTableContent.Add(rowData);
                }
            }

            table.Content = newTableContent;
            return table;
        } // figure out a better way to do this LOL

        public static Table WtihCodeblock(this Table table, int fieldIndex)
        {
            foreach(object[] rowData in table.Content)
            {
                rowData[fieldIndex] = $"`{(string)rowData[fieldIndex]}`";
            }
            return table;
        }

        // too lazy to implement sorting algos LOL
        // public static Table WithOrder(this Table table)
        // {

        // }
    }
}