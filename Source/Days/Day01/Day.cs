namespace Day01;


class Day : BaseDay
{
    public override string Run(int part, string rawData)
    {
        int position = 50;
        int count = 0;
        foreach (int value in rawData.Lines().Select(line => line[0] == 'L' ? -int.Parse(line[1..]) : int.Parse(line[1..])))
        {
            if (part == 2)
            {
                int full_rotations = Math.Abs(value) / 100;
                count += full_rotations;

                int remainder = value % 100;

                if (position != 0)
                {
                    if ((remainder < 0 && position <= -remainder) ||
                        (remainder > 0 && position + remainder >= 100))
                    {
                        count++;
                    }
                }
            }
            position = (position + value) % 100;

            if (position < 0)
            {
                position += 100;
            }

            if (position == 0 && part == 1)
            {
                count++;
            }

        }
        return count.ToString();
    }
}
