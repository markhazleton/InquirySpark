"""
Apply conversation migration to conversation-dev.db.
Uses Python built-in sqlite3 module (no external packages).
"""
import sqlite3
import sys

db_path = sys.argv[1] if len(sys.argv) > 1 else r"data\sqlite\conversation-dev.db"
db = sqlite3.connect(db_path)
c = db.cursor()

survey_cols = [x[1] for x in c.execute("PRAGMA table_info(SurveyResponse)").fetchall()]
if "ConversationId" not in survey_cols:
    c.execute("ALTER TABLE SurveyResponse ADD COLUMN ConversationId TEXT NULL")
    print("+ Added ConversationId to SurveyResponse")
else:
    print("~ ConversationId already exists")

indexes = [x[0] for x in c.execute("SELECT name FROM sqlite_master WHERE type='index'").fetchall()]
if "IX_SurveyResponse_ConversationId" not in indexes:
    c.execute(
        "CREATE UNIQUE INDEX IX_SurveyResponse_ConversationId "
        "ON SurveyResponse (ConversationId) WHERE ConversationId IS NOT NULL"
    )
    print("+ Created partial unique index IX_SurveyResponse_ConversationId")
else:
    print("~ Index already exists")

user_cols = [x[1] for x in c.execute("PRAGMA table_info(ApplicationUser)").fetchall()]
if "PasswordHash" not in user_cols:
    c.execute("ALTER TABLE ApplicationUser ADD COLUMN PasswordHash TEXT NULL")
    print("+ Added PasswordHash to ApplicationUser")
else:
    print("~ PasswordHash already exists")

db.commit()
db.close()
print("Done.")
