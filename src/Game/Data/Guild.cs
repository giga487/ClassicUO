using System;

/* giga487, inizializzaizone classe */
public class Guild
{
    uint counterMember = 0;
    uint actual_index = 0;

    public Guild(uint counter)
	{
        counterMember = counter;
        string* member = new string[counterMember];
        uint* serialMember = new uint[counterMember];

    }

    public AddMember(string memberName, uint serialM)
    {
        member[actual_index] = memberName;
        serialMember[actual_index] = serialM;
        actual_index++;
    }

    public RemoveMember(uint serialM)
    {
        
    }
}
