====================================================
Exam 02 - Examination System
====================================================

Design notes:
- Question is an abstract base class. TrueFalseQuestion and MCQQuestion inherit from it.
  MCQQuestion is reused for both the Final exam (MCQ) and the Practical exam (MCQ).
- Exam is an abstract base class. FinalExam and PracticalExam inherit from it and give
  their own version of ShowExam().
- Subject holds one Exam and has a method to create/assign it.


----------------------------------------------------
Answer class
----------------------------------------------------
using System;

class Answer
{
    public int AnswerId { get; set; }
    public string AnswerText { get; set; }

    public Answer(int answerId, string answerText)
    {
        AnswerId = answerId;
        AnswerText = answerText;
    }

    public override string ToString()
    {
        return $"[{AnswerId}] {AnswerText}";
    }
}


----------------------------------------------------
Base Question class
----------------------------------------------------
abstract class Question : ICloneable, IComparable<Question>
{
    public string Header { get; set; }
    public string Body { get; set; }
    public int Mark { get; set; }
    public Answer[] AnswerList { get; set; }
    public Answer RightAnswer { get; set; }

    protected Question(string header, string body, int mark, Answer[] answerList, Answer rightAnswer)
    {
        Header = header;
        Body = body;
        Mark = mark;
        AnswerList = answerList;
        RightAnswer = rightAnswer;
    }

    // each question type shows itself differently
    public abstract void ShowQuestion();

    public abstract object Clone();

    public int CompareTo(Question other)
    {
        return Mark.CompareTo(other.Mark);
    }

    public override string ToString()
    {
        return $"Header: {Header} - Body: {Body} - Mark: {Mark}";
    }
}


----------------------------------------------------
TrueFalseQuestion (Final exam)
----------------------------------------------------
class TrueFalseQuestion : Question
{
    public TrueFalseQuestion(string header, string body, int mark, Answer[] answerList, Answer rightAnswer)
        : base(header, body, mark, answerList, rightAnswer)
    {
    }

    public override void ShowQuestion()
    {
        Console.WriteLine(this);
        Console.WriteLine("Choose: True or False");
        foreach (var ans in AnswerList)
            Console.WriteLine(ans);
    }

    public override object Clone()
    {
        return new TrueFalseQuestion(Header, Body, Mark, AnswerList, RightAnswer);
    }
}


----------------------------------------------------
MCQQuestion (used by Final exam and Practical exam)
----------------------------------------------------
class MCQQuestion : Question
{
    public MCQQuestion(string header, string body, int mark, Answer[] answerList, Answer rightAnswer)
        : base(header, body, mark, answerList, rightAnswer)
    {
    }

    public override void ShowQuestion()
    {
        Console.WriteLine(this);
        foreach (var ans in AnswerList)
            Console.WriteLine(ans);
    }

    public override object Clone()
    {
        return new MCQQuestion(Header, Body, Mark, AnswerList, RightAnswer);
    }
}


----------------------------------------------------
Base Exam class
----------------------------------------------------
abstract class Exam
{
    public DateTime Time { get; set; }
    public int NumberOfQuestions { get; set; }
    public Question[] Questions { get; set; }

    protected Exam(DateTime time, int numberOfQuestions, Question[] questions)
    {
        Time = time;
        NumberOfQuestions = numberOfQuestions;
        Questions = questions;
    }

    // every exam type shows itself differently
    public abstract void ShowExam();
}


----------------------------------------------------
FinalExam - shows questions, answers and grade
----------------------------------------------------
class FinalExam : Exam
{
    public FinalExam(DateTime time, int numberOfQuestions, Question[] questions)
        : base(time, numberOfQuestions, questions)
    {
    }

    public override void ShowExam()
    {
        Console.WriteLine("----- Final Exam -----");
        int totalGrade = 0;

        foreach (var q in Questions)
        {
            q.ShowQuestion();
            totalGrade += q.Mark;
        }

        Console.WriteLine($"Total Grade: {totalGrade}");
    }
}


----------------------------------------------------
PracticalExam - shows the right answer after finishing
----------------------------------------------------
class PracticalExam : Exam
{
    public PracticalExam(DateTime time, int numberOfQuestions, Question[] questions)
        : base(time, numberOfQuestions, questions)
    {
    }

    public override void ShowExam()
    {
        Console.WriteLine("----- Practical Exam -----");

        foreach (var q in Questions)
        {
            q.ShowQuestion();
            Console.WriteLine("Right Answer: " + q.RightAnswer);
        }
    }
}


----------------------------------------------------
Subject class
----------------------------------------------------
class Subject
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; }
    public Exam SubjectExam { get; set; }

    public Subject(int subjectId, string subjectName)
    {
        SubjectId = subjectId;
        SubjectName = subjectName;
    }

    public void CreateExam(Exam exam)
    {
        SubjectExam = exam;
    }

    public override string ToString()
    {
        return $"Subject Id: {SubjectId} - Subject Name: {SubjectName}";
    }
}


----------------------------------------------------
Main - build a subject and run one exam type
----------------------------------------------------
class Program
{
    static void Main()
    {
        // ---- Final Exam: True/False question ----
        Answer tf1 = new Answer(1, "True");
        Answer tf2 = new Answer(2, "False");
        Answer[] tfAnswers = { tf1, tf2 };

        Question q1 = new TrueFalseQuestion("Q1", "C# is a type-safe language", 5, tfAnswers, tf1);

        // ---- Final Exam: MCQ question ----
        Answer m1 = new Answer(1, "Compile-time");
        Answer m2 = new Answer(2, "Run-time");
        Answer m3 = new Answer(3, "Design-time");
        Answer[] mcqAnswers = { m1, m2, m3 };

        Question q2 = new MCQQuestion("Q2", "When is 'var' type resolved?", 5, mcqAnswers, m1);

        Question[] finalQuestions = { q1, q2 };

        Exam finalExam = new FinalExam(DateTime.Now, finalQuestions.Length, finalQuestions);

        Subject subject = new Subject(101, "OOP");
        subject.CreateExam(finalExam);

        Console.WriteLine(subject);
        subject.SubjectExam.ShowExam();

        // ---- To test a Practical Exam instead, build MCQ questions and do: ----
        // Exam practicalExam = new PracticalExam(DateTime.Now, practicalQuestions.Length, practicalQuestions);
        // subject.CreateExam(practicalExam);
        // subject.SubjectExam.ShowExam();
    }
}
