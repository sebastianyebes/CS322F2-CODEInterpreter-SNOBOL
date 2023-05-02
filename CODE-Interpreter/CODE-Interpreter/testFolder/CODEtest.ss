BEGIN CODE
	BOOL a = "TRUE", b = "FALSE", c = "TRUE", d
	IF(a)
	BEGIN IF
	    a = "FALSE"
	    IF(NOT a)
	    BEGIN IF
	        DISPLAY: "working1"
	    END IF
	END IF
	ELSE
	BEGIN IF
        a = "FALSE"
        IF(NOT a)
        BEGIN IF
            DISPLAY: "working2"
        END IF
    END IF
END CODE