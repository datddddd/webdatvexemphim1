PGDMP      &                }            movie    17.2    17.2 7    1           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                           false            2           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                           false            3           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                           false            4           1262    24577    movie    DATABASE     �   CREATE DATABASE movie WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'English_United States.1252';
    DROP DATABASE movie;
                     postgres    false            �            1255    24746    set_created_at()    FUNCTION     �   CREATE FUNCTION public.set_created_at() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF NEW.created_at IS NULL THEN
        NEW.created_at := NOW();
    END IF;
    RETURN NEW;
END;
$$;
 '   DROP FUNCTION public.set_created_at();
       public               postgres    false            �            1259    24663    Genres    TABLE     h   CREATE TABLE public."Genres" (
    "Id" integer NOT NULL,
    "Name" character varying(255) NOT NULL
);
    DROP TABLE public."Genres";
       public         heap r       postgres    false            �            1259    24698    Movie    TABLE        CREATE TABLE public."Movie" (
    "Id" integer NOT NULL,
    "MovieName" character varying(255) NOT NULL,
    "Capacity" integer NOT NULL,
    "Status" character varying(255) DEFAULT 'Ongoing'::character varying NOT NULL,
    "MovieImage" text,
    "CreateAt" timestamp without time zone DEFAULT now(),
    "GenreId" integer NOT NULL,
    "Descripton" character varying(9999999),
    "MovieImage2" text,
    "Director" character varying(255),
    "Video" character varying(500),
    CONSTRAINT movies_capacity_check CHECK (("Capacity" > 0))
);
    DROP TABLE public."Movie";
       public         heap r       postgres    false            �            1259    32912    Seat    TABLE     �   CREATE TABLE public."Seat" (
    "Id" integer NOT NULL,
    "SeatNumber" integer NOT NULL,
    "IsAvailable" boolean NOT NULL,
    "ShowtimeId" integer NOT NULL
);
    DROP TABLE public."Seat";
       public         heap r       postgres    false            �            1259    32898    Showtime    TABLE     �   CREATE TABLE public."Showtime" (
    "Id" integer NOT NULL,
    "MovieId" integer NOT NULL,
    "Date" date NOT NULL,
    "ShowTimes" time without time zone[] NOT NULL,
    "Price" numeric(10,2) NOT NULL,
    "Capacity" integer
);
    DROP TABLE public."Showtime";
       public         heap r       postgres    false            �            1259    32941    Ticket    TABLE       CREATE TABLE public."Ticket" (
    "Id" integer NOT NULL,
    "SeatId" integer NOT NULL,
    "ShowtimeId" integer NOT NULL,
    "UserId" integer NOT NULL,
    "BookingDate" timestamp without time zone NOT NULL,
    "IsPaid" boolean NOT NULL,
    "Price" numeric(10,2) NOT NULL
);
    DROP TABLE public."Ticket";
       public         heap r       postgres    false            �            1259    24649    Users    TABLE     &  CREATE TABLE public."Users" (
    "Id" integer NOT NULL,
    "Name" character varying(255),
    "Username" character varying(100) NOT NULL,
    "Password" text NOT NULL,
    "Email" character varying(255) NOT NULL,
    "Role" character varying(50) DEFAULT 'User'::character varying NOT NULL
);
    DROP TABLE public."Users";
       public         heap r       postgres    false            �            1259    24662    genres_id_seq    SEQUENCE     �   CREATE SEQUENCE public.genres_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 $   DROP SEQUENCE public.genres_id_seq;
       public               postgres    false    220            5           0    0    genres_id_seq    SEQUENCE OWNED BY     C   ALTER SEQUENCE public.genres_id_seq OWNED BY public."Genres"."Id";
          public               postgres    false    219            �            1259    24697    movies_id_seq    SEQUENCE     �   CREATE SEQUENCE public.movies_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 $   DROP SEQUENCE public.movies_id_seq;
       public               postgres    false    222            6           0    0    movies_id_seq    SEQUENCE OWNED BY     B   ALTER SEQUENCE public.movies_id_seq OWNED BY public."Movie"."Id";
          public               postgres    false    221            �            1259    32911    seat_id_seq    SEQUENCE     �   CREATE SEQUENCE public.seat_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 "   DROP SEQUENCE public.seat_id_seq;
       public               postgres    false    226            7           0    0    seat_id_seq    SEQUENCE OWNED BY     ?   ALTER SEQUENCE public.seat_id_seq OWNED BY public."Seat"."Id";
          public               postgres    false    225            �            1259    32897    showtime_id_seq    SEQUENCE     �   CREATE SEQUENCE public.showtime_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 &   DROP SEQUENCE public.showtime_id_seq;
       public               postgres    false    224            8           0    0    showtime_id_seq    SEQUENCE OWNED BY     G   ALTER SEQUENCE public.showtime_id_seq OWNED BY public."Showtime"."Id";
          public               postgres    false    223            �            1259    32940    ticket_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ticket_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 $   DROP SEQUENCE public.ticket_id_seq;
       public               postgres    false    228            9           0    0    ticket_id_seq    SEQUENCE OWNED BY     C   ALTER SEQUENCE public.ticket_id_seq OWNED BY public."Ticket"."Id";
          public               postgres    false    227            �            1259    24648    users_id_seq    SEQUENCE     �   CREATE SEQUENCE public.users_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 #   DROP SEQUENCE public.users_id_seq;
       public               postgres    false    218            :           0    0    users_id_seq    SEQUENCE OWNED BY     A   ALTER SEQUENCE public.users_id_seq OWNED BY public."Users"."Id";
          public               postgres    false    217            t           2604    24666 	   Genres Id    DEFAULT     j   ALTER TABLE ONLY public."Genres" ALTER COLUMN "Id" SET DEFAULT nextval('public.genres_id_seq'::regclass);
 <   ALTER TABLE public."Genres" ALTER COLUMN "Id" DROP DEFAULT;
       public               postgres    false    220    219    220            u           2604    24701    Movie Id    DEFAULT     i   ALTER TABLE ONLY public."Movie" ALTER COLUMN "Id" SET DEFAULT nextval('public.movies_id_seq'::regclass);
 ;   ALTER TABLE public."Movie" ALTER COLUMN "Id" DROP DEFAULT;
       public               postgres    false    221    222    222            y           2604    32915    Seat Id    DEFAULT     f   ALTER TABLE ONLY public."Seat" ALTER COLUMN "Id" SET DEFAULT nextval('public.seat_id_seq'::regclass);
 :   ALTER TABLE public."Seat" ALTER COLUMN "Id" DROP DEFAULT;
       public               postgres    false    225    226    226            x           2604    32901    Showtime Id    DEFAULT     n   ALTER TABLE ONLY public."Showtime" ALTER COLUMN "Id" SET DEFAULT nextval('public.showtime_id_seq'::regclass);
 >   ALTER TABLE public."Showtime" ALTER COLUMN "Id" DROP DEFAULT;
       public               postgres    false    223    224    224            z           2604    32944 	   Ticket Id    DEFAULT     j   ALTER TABLE ONLY public."Ticket" ALTER COLUMN "Id" SET DEFAULT nextval('public.ticket_id_seq'::regclass);
 <   ALTER TABLE public."Ticket" ALTER COLUMN "Id" DROP DEFAULT;
       public               postgres    false    227    228    228            r           2604    24652    Users Id    DEFAULT     h   ALTER TABLE ONLY public."Users" ALTER COLUMN "Id" SET DEFAULT nextval('public.users_id_seq'::regclass);
 ;   ALTER TABLE public."Users" ALTER COLUMN "Id" DROP DEFAULT;
       public               postgres    false    218    217    218            &          0    24663    Genres 
   TABLE DATA           0   COPY public."Genres" ("Id", "Name") FROM stdin;
    public               postgres    false    220   @       (          0    24698    Movie 
   TABLE DATA           �   COPY public."Movie" ("Id", "MovieName", "Capacity", "Status", "MovieImage", "CreateAt", "GenreId", "Descripton", "MovieImage2", "Director", "Video") FROM stdin;
    public               postgres    false    222   S@       ,          0    32912    Seat 
   TABLE DATA           Q   COPY public."Seat" ("Id", "SeatNumber", "IsAvailable", "ShowtimeId") FROM stdin;
    public               postgres    false    226   �D       *          0    32898    Showtime 
   TABLE DATA           _   COPY public."Showtime" ("Id", "MovieId", "Date", "ShowTimes", "Price", "Capacity") FROM stdin;
    public               postgres    false    224   �J       .          0    32941    Ticket 
   TABLE DATA           l   COPY public."Ticket" ("Id", "SeatId", "ShowtimeId", "UserId", "BookingDate", "IsPaid", "Price") FROM stdin;
    public               postgres    false    228   K       $          0    24649    Users 
   TABLE DATA           X   COPY public."Users" ("Id", "Name", "Username", "Password", "Email", "Role") FROM stdin;
    public               postgres    false    218   �K       ;           0    0    genres_id_seq    SEQUENCE SET     ;   SELECT pg_catalog.setval('public.genres_id_seq', 4, true);
          public               postgres    false    219            <           0    0    movies_id_seq    SEQUENCE SET     <   SELECT pg_catalog.setval('public.movies_id_seq', 24, true);
          public               postgres    false    221            =           0    0    seat_id_seq    SEQUENCE SET     ;   SELECT pg_catalog.setval('public.seat_id_seq', 700, true);
          public               postgres    false    225            >           0    0    showtime_id_seq    SEQUENCE SET     =   SELECT pg_catalog.setval('public.showtime_id_seq', 9, true);
          public               postgres    false    223            ?           0    0    ticket_id_seq    SEQUENCE SET     <   SELECT pg_catalog.setval('public.ticket_id_seq', 64, true);
          public               postgres    false    227            @           0    0    users_id_seq    SEQUENCE SET     ;   SELECT pg_catalog.setval('public.users_id_seq', 29, true);
          public               postgres    false    217            �           2606    24668    Genres genres_pkey 
   CONSTRAINT     T   ALTER TABLE ONLY public."Genres"
    ADD CONSTRAINT genres_pkey PRIMARY KEY ("Id");
 >   ALTER TABLE ONLY public."Genres" DROP CONSTRAINT genres_pkey;
       public                 postgres    false    220            �           2606    24708    Movie movies_pkey 
   CONSTRAINT     S   ALTER TABLE ONLY public."Movie"
    ADD CONSTRAINT movies_pkey PRIMARY KEY ("Id");
 =   ALTER TABLE ONLY public."Movie" DROP CONSTRAINT movies_pkey;
       public                 postgres    false    222            �           2606    32917    Seat seat_pkey 
   CONSTRAINT     P   ALTER TABLE ONLY public."Seat"
    ADD CONSTRAINT seat_pkey PRIMARY KEY ("Id");
 :   ALTER TABLE ONLY public."Seat" DROP CONSTRAINT seat_pkey;
       public                 postgres    false    226            �           2606    32905    Showtime showtime_pkey 
   CONSTRAINT     X   ALTER TABLE ONLY public."Showtime"
    ADD CONSTRAINT showtime_pkey PRIMARY KEY ("Id");
 B   ALTER TABLE ONLY public."Showtime" DROP CONSTRAINT showtime_pkey;
       public                 postgres    false    224            �           2606    32946    Ticket ticket_pkey 
   CONSTRAINT     T   ALTER TABLE ONLY public."Ticket"
    ADD CONSTRAINT ticket_pkey PRIMARY KEY ("Id");
 >   ALTER TABLE ONLY public."Ticket" DROP CONSTRAINT ticket_pkey;
       public                 postgres    false    228            }           2606    24660    Users users_email_key 
   CONSTRAINT     U   ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT users_email_key UNIQUE ("Email");
 A   ALTER TABLE ONLY public."Users" DROP CONSTRAINT users_email_key;
       public                 postgres    false    218                       2606    24656    Users users_pkey 
   CONSTRAINT     R   ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT users_pkey PRIMARY KEY ("Id");
 <   ALTER TABLE ONLY public."Users" DROP CONSTRAINT users_pkey;
       public                 postgres    false    218            �           2606    24658    Users users_username_key 
   CONSTRAINT     [   ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT users_username_key UNIQUE ("Username");
 D   ALTER TABLE ONLY public."Users" DROP CONSTRAINT users_username_key;
       public                 postgres    false    218            �           2606    32906    Showtime fk_movie    FK CONSTRAINT     �   ALTER TABLE ONLY public."Showtime"
    ADD CONSTRAINT fk_movie FOREIGN KEY ("MovieId") REFERENCES public."Movie"("Id") ON DELETE CASCADE;
 =   ALTER TABLE ONLY public."Showtime" DROP CONSTRAINT fk_movie;
       public               postgres    false    4741    222    224            �           2606    24711    Movie fk_movies_genre    FK CONSTRAINT     �   ALTER TABLE ONLY public."Movie"
    ADD CONSTRAINT fk_movies_genre FOREIGN KEY ("GenreId") REFERENCES public."Genres"("Id") ON DELETE CASCADE;
 A   ALTER TABLE ONLY public."Movie" DROP CONSTRAINT fk_movies_genre;
       public               postgres    false    222    4739    220            �           2606    32947    Ticket fk_seat    FK CONSTRAINT     �   ALTER TABLE ONLY public."Ticket"
    ADD CONSTRAINT fk_seat FOREIGN KEY ("SeatId") REFERENCES public."Seat"("Id") ON DELETE CASCADE;
 :   ALTER TABLE ONLY public."Ticket" DROP CONSTRAINT fk_seat;
       public               postgres    false    226    228    4745            �           2606    32918    Seat fk_showtime    FK CONSTRAINT     �   ALTER TABLE ONLY public."Seat"
    ADD CONSTRAINT fk_showtime FOREIGN KEY ("ShowtimeId") REFERENCES public."Showtime"("Id") ON DELETE CASCADE;
 <   ALTER TABLE ONLY public."Seat" DROP CONSTRAINT fk_showtime;
       public               postgres    false    226    224    4743            �           2606    32952    Ticket fk_showtime    FK CONSTRAINT     �   ALTER TABLE ONLY public."Ticket"
    ADD CONSTRAINT fk_showtime FOREIGN KEY ("ShowtimeId") REFERENCES public."Showtime"("Id") ON DELETE CASCADE;
 >   ALTER TABLE ONLY public."Ticket" DROP CONSTRAINT fk_showtime;
       public               postgres    false    4743    228    224            �           2606    32957    Ticket fk_user    FK CONSTRAINT     �   ALTER TABLE ONLY public."Ticket"
    ADD CONSTRAINT fk_user FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE;
 :   ALTER TABLE ONLY public."Ticket" DROP CONSTRAINT fk_user;
       public               postgres    false    228    4735    218            &   %   x�3�tL.����2�t��MM��2�t˄��qqq ��	      (   �  x��U]k�F}����Zؕw��GLp6�v��I��vV�L��Q�Ѯ7o�B)�O!�xm�q[�v)�(yP����;#ic;�K^�5s�=��sGM����i�-<�]��F��<������}���2�rf�2}��:�*��Ѱ��n0��e�pW0���x�Ȫ[3�z�Vo�e��-k�l����Z��]�*�b�YD�p�	B�s.�}6m�2X��5߲�f�~yfnq/�-hGY�@��,~�a5=�v��A��k�x�2B��!%
4���
d��@�d߆�v0p��zL�Ux���e��|�ⱀ˒�J���"�T������(�	W'�6�=��*Gr��@�b�#8M�=(��D$�j���Gd4K~��,���}�`=��Hi�c܉�1'�P�{��"4�^ S�d�+�ÍW�L>,�(�t�#�t�1`��P���D�	�tw�]�ëRq�H�&�����DH�}^�P�mm��/�@�S�tɥ����a�Qɵ5�/No"�_Z���$�K�T}ʩ���G%����b�t��X�5Rjh�7���7��7��C����e��+�e�%O`��@?�(�M�7��
�,y���4�j�U����.m�u���xU��A[�3�!�̀*7>#UU���/��9Pa��i����o�~T *]q���«zVU_��d�܃h�6�h�]UIc�n����(��B�.b���;��3=�@(�R����}��j�=��h��K�Ԇ���S%H����U�tq���ݹ)<;N�2��x	�4�­hW�!>�n[�<!h��pҀ��oc?�p��$�,5	�2��q�a)�|���ѡ�¢J���8�G]γ��_D���GF���0�'�ק�	_�&���3S�a#�Iq���GIB��:+Iw#,R(��x�B��h�ʁ�̮�b�	�A5(���t��DE�5��dӆ31��M��=�#=EA�)K~R/��/��?��_ۼF��?�{�̰Q0�&�'��~ؚ���HD2�:&��Ж�^,�.վ����^�XM��A$���ReI�.��hI�Xoje����F2X�gmc�~�F�+��Ew�����]`Cߺ��K>�-,{�5G޴��̟
��	y�<�B��ok�r�cV*�� �E�k      ,   �  x�=�[n�@�ˇY����]�{N���f�Qd `�#�
�s=���b�=WEA#4cB+�cC'�Gy�>���ɑ9"93g$����ܑ�'�s�+nν��9���9�qs�=���{�͹���s�7�>W<��d���z|������a����xX{V<�=;֞k�QL��D=QL�^z&jD1Q3��ZQLԎb�N����q�`m<1X���w��1c�6V�Ǝ��81X�WL�f�dm�1Y�OL�f�dm���M�b�6WL����<1Y[W,�V�bmݱX[O,�V�bm�X������c�֎��:�X�Wl�v�fm߱Y���=b3�gl&�����]ab��L�+'�0q�8L�'O�T�Έ�ڙqX;+kG����S��-�a���uU��uW��^�m���zmX��b�"���MvS�ɮ�"ʊ�eG ��$�D�h��	<Q��(Y"1E�ڽٽ�{�+}$
)�#qHI �DJI,RRH���C��$���d��$%�$*)y$qI=6"�2Ib��J��\���d��"%�$��V�%�$N)	%�J�(�U��_���+%�$b)Y%1KI+�ZJ^I�RK"��Y��Ԓ���/5�z���D+%�$^)I%K�*�YJZI�R�J▒X��̒إ��D/%�$~��ve��0%�$�)�%qLI0�dJ�I,SRL���cϔ$���d��4%�$���O7v%�D6%�$�)�&�M�5�oJ�I�S[��]�'�OI=�����v���{A߃u���;����͏���wKFK�VRZ�������Hj��JVK�V�Z������Ėȭd��n%�%z+�-�[In��JO,�Ǖ��˙зP?���߯������ts������<G�5��k��<��yL_鈺t�������~H��#�c����v�߷���m��2�BFR�H
I!#)d$���BFR�H
I!#)d$�����2�BFRH��REF��H�"#Ud���T��*2RE®"#Ud$�����2�BFR�H
I!a���2�BFR�H
I!#)d$z�=F��H��#�c$z�D���1=F�G��c$z�D�i�#�c����1�?��#�c����1�?F��H�_�}H
���K!s���S��)d�2w
�;�̝B�N!s����B�N!s���S��)d�2w
�;�̝B�N!�/�̝B�N!s���S��)d�2w
�;�̝B�_
�;�̝B�N!s���S��)d�d�qg���H��"sg�����Yd�2w
�;�̝B�N!�/�̝B�N!s���� �����Yd�,2w�;�Ŀ,2w�;�̝E��"sg�����Yd��(�{��9�"Qa�B��S������˗]e�����]e�2wW�����U��*sw�����]%�u�����]e�2wW����]@�. s��H�+ s����d�2w����]@�. s��[�������K��>�������)��      *   B   x�]ʻ�0�x�[s��صЉE�8 B�l^�w��d�4��h*�K\J>�j�ٖ����F���      .   �   x��ѻq1�W�X�C�Ǯ���qv ��u�} �5%�A������7 )܂�>�3sc>,*/g~>�q�H����DEW0
�_�� �Lf�;�To��8��^ �d���l�h��+0���>p���x�$O��P�=�:�
��qް��P`4���+�� 4HM�'�Ғ�Y�+p�W��>�SiVp���SW�P���9�VP=���KP=���+�ю���^�+      $     x�m�I��@��x�Z�FeW����Q�Q1�2�p_�>B�5������Z�atUDFF�����E%B었h�N���d-k�,v��3"_0V��<J	�Qjn�ʜR�"p=s�w��ã'�.=��/Z�{��}�/���XB���G�h�78=0i�X�?���id���A�i憜��x0.��.�;k����^��+�tVirm6x�{�^lz�fz�I?�♮�џr�/T�A����S��v�'u�Y1-G��ԅĚaql{uD)?^��"?*��`C�7><�؃��������m)�M�2�������&�,RQ,o�`����RS��Em�]���"�����^��g��=LV���^�r�UU��i�S�A�D�J����.�9s�xER�@J!�> �tm���O���[v��᫤G��b����U��;���|{����~P��&�!����Q��q�c��o�S�r����m?�gAg�N��x�/d�������     